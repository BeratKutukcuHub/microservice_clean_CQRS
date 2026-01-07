using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AbstractionBlocks.Common.Messaging.Configuration;
using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Messaging.Interfaces;

namespace AbstractionBlocks.Common.Messaging.RabbitMQ;

public class RabbitMQEventBus : IEventBus, IAsyncDisposable
{
    private readonly RabbitMQConnection _connection;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQEventBus> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, List<Type>> _handlers;
    private readonly Dictionary<string, IChannel> _consumerChannels;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private bool _disposed;

    public RabbitMQEventBus(
        RabbitMQConnection connection,
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQEventBus> logger,
        IServiceProvider serviceProvider)
    {
        _connection = connection;
        _settings = settings.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _handlers = new Dictionary<string, List<Type>>();
        _consumerChannels = new Dictionary<string, IChannel>();
    }

    public async Task InitializeAsync()
    {
        await _connection.ConnectAsync();
        await CreateExchangeAsync();
    }

    private async Task CreateExchangeAsync()
    {
        var channel = await _connection.CreateChannelAsync();
        
        try
        {
            await channel.ExchangeDeclareAsync(
                exchange: _settings.ExchangeName,
                type: _settings.ExchangeType,
                durable: _settings.ExchangeDurable,
                autoDelete: _settings.ExchangeAutoDelete);

            if (_settings.EnableDeadLetterExchange)
            {
                await channel.ExchangeDeclareAsync(
                    exchange: _settings.DeadLetterExchangeName,
                    type: "topic",
                    durable: true,
                    autoDelete: false);
            }

            _logger.LogInformation("RabbitMQ exchange '{ExchangeName}' created", _settings.ExchangeName);
        }
        finally
        {
            await channel.CloseAsync();
            await channel.DisposeAsync();
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IntegrationEvent
    {
        var routingKey = GetRoutingKey<TEvent>();
        await PublishAsync(@event, routingKey, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default) 
        where TEvent : IntegrationEvent
    {
        if (!_connection.IsConnected)
        {
            await _connection.ConnectAsync();
        }

        var channel = await _connection.CreateChannelAsync();
        
        try
        {
            var envelope = new EventEnvelope<TEvent>(@event);
            var message = JsonSerializer.Serialize(envelope, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var body = Encoding.UTF8.GetBytes(message);

            var properties = new BasicProperties
            {
                Persistent = true,
                MessageId = envelope.MessageId.ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                ContentType = "application/json",
                Type = envelope.MessageType,
                Headers = new Dictionary<string, object?>
                {
                    { "event-type", @event.EventType },
                    { "event-version", @event.EventVersion },
                    { "source", @event.Source }
                }
            };

            if (@event.CorrelationId.HasValue)
            {
                properties.CorrelationId = @event.CorrelationId.Value.ToString();
            }

            await channel.BasicPublishAsync(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Published event {EventType} with ID {EventId} to routing key {RoutingKey}",
                @event.EventType, @event.EventId, routingKey);
        }
        finally
        {
            await channel.CloseAsync();
            await channel.DisposeAsync();
        }
    }

    public void Subscribe<TEvent, THandler>() 
        where TEvent : IntegrationEvent 
        where THandler : IEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        var handlerType = typeof(THandler);

        _lock.Wait();
        try
        {
            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new List<Type>();
            }

            if (_handlers[eventName].Contains(handlerType))
            {
                _logger.LogWarning(
                    "Handler {HandlerType} already registered for event {EventName}",
                    handlerType.Name, eventName);
                return;
            }

            _handlers[eventName].Add(handlerType);
        }
        finally
        {
            _lock.Release();
        }

        Task.Run(async () => await StartBasicConsumeAsync<TEvent>());
        
        _logger.LogInformation(
            "Subscribed {HandlerType} to event {EventName}",
            handlerType.Name, eventName);
    }

    public void Unsubscribe<TEvent, THandler>() 
        where TEvent : IntegrationEvent 
        where THandler : IEventHandler<TEvent>
    {
        var eventName = typeof(TEvent).Name;
        var handlerType = typeof(THandler);

        _lock.Wait();
        try
        {
            if (_handlers.ContainsKey(eventName))
            {
                _handlers[eventName].Remove(handlerType);
                
                if (_handlers[eventName].Count == 0)
                {
                    _handlers.Remove(eventName);
                    
                    if (_consumerChannels.TryGetValue(eventName, out var channel))
                    {
                        Task.Run(async () =>
                        {
                            await channel.CloseAsync();
                            await channel.DisposeAsync();
                        });
                        _consumerChannels.Remove(eventName);
                    }
                }
            }
        }
        finally
        {
            _lock.Release();
        }

        _logger.LogInformation(
            "Unsubscribed {HandlerType} from event {EventName}",
            handlerType.Name, eventName);
    }

    private async Task StartBasicConsumeAsync<TEvent>() where TEvent : IntegrationEvent
    {
        var eventName = typeof(TEvent).Name;

        await _lock.WaitAsync();
        try
        {
            if (_consumerChannels.ContainsKey(eventName))
            {
                return;
            }

            if (!_connection.IsConnected)
            {
                await _connection.ConnectAsync();
            }

            var channel = await _connection.CreateChannelAsync();
            await channel.BasicQosAsync(prefetchSize: 0, prefetchCount: _settings.PrefetchCount, global: false);

            var queueName = $"{_settings.QueuePrefix}{eventName}";
            var routingKey = GetRoutingKey<TEvent>();

            var queueArgs = new Dictionary<string, object?>();
            
            if (_settings.EnableDeadLetterExchange)
            {
                queueArgs.Add("x-dead-letter-exchange", _settings.DeadLetterExchangeName);
                queueArgs.Add("x-dead-letter-routing-key", $"dlx.{routingKey}");
            }

            if (_settings.MessageTTL > 0)
            {
                queueArgs.Add("x-message-ttl", _settings.MessageTTL);
            }

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: _settings.QueueDurable,
                exclusive: _settings.QueueExclusive,
                autoDelete: _settings.QueueAutoDelete,
                arguments: queueArgs);

            await channel.QueueBindAsync(
                queue: queueName,
                exchange: _settings.ExchangeName,
                routingKey: routingKey);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                await ProcessEventAsync<TEvent>(eventArgs, channel);
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer);

            _consumerChannels[eventName] = channel;

            _logger.LogInformation(
                "Started consuming queue {QueueName} with routing key {RoutingKey}",
                queueName, routingKey);
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task ProcessEventAsync<TEvent>(BasicDeliverEventArgs eventArgs, IChannel channel) 
        where TEvent : IntegrationEvent
    {
        var eventName = typeof(TEvent).Name;
        var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());

        try
        {
            var envelope = JsonSerializer.Deserialize<EventEnvelope<TEvent>>(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            if (envelope?.Payload == null)
            {
                _logger.LogWarning("Failed to deserialize event {EventName}", eventName);
                await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            await ProcessEventHandlersAsync(eventName, envelope.Payload);
            
            await channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            
            _logger.LogInformation(
                "Successfully processed event {EventName} with ID {EventId}",
                eventName, envelope.Payload.EventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Error processing event {EventName}. Message: {Message}",
                eventName, message);
            
            await channel.BasicNackAsync(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
    }

    private async Task ProcessEventHandlersAsync<TEvent>(string eventName, TEvent @event) 
        where TEvent : IntegrationEvent
    {
        if (!_handlers.ContainsKey(eventName))
        {
            _logger.LogWarning("No handlers registered for event {EventName}", eventName);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        
        foreach (var handlerType in _handlers[eventName])
        {
            var handler = scope.ServiceProvider.GetService(handlerType) as IEventHandler<TEvent>;
            
            if (handler == null)
            {
                _logger.LogWarning(
                    "Handler {HandlerType} not found in service provider",
                    handlerType.Name);
                continue;
            }

            await handler.HandleAsync(@event);
        }
    }

    private string GetRoutingKey<TEvent>() where TEvent : IntegrationEvent
    {
        var eventType = typeof(TEvent);
        var eventName = eventType.Name;
        
        // Format: service.entity.action (e.g., product.product.created)
        var parts = eventName.Split(new[] { "Event" }, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0].ToLowerInvariant() : eventName.ToLowerInvariant();
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        foreach (var channel in _consumerChannels.Values)
        {
            try
            {
                await channel.CloseAsync();
                await channel.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing consumer channel");
            }
        }

        _consumerChannels.Clear();
        _handlers.Clear();
        _lock.Dispose();
    }
}
