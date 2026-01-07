using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AbstractionBlocks.Common.Messaging.Configuration;

namespace AbstractionBlocks.Common.Messaging.RabbitMQ;

public class RabbitMQConnection : IAsyncDisposable
{
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQConnection> _logger;
    private IConnection? _connection;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private bool _disposed;
    private readonly ResiliencePipeline _retryPipeline;

    public RabbitMQConnection(
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQConnection> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        
        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = _settings.RetryCount,
                Delay = TimeSpan.FromMilliseconds(_settings.RetryDelayMilliseconds),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    _logger.LogWarning("Retry attempt {Attempt} after {Delay}ms", 
                        args.AttemptNumber, args.RetryDelay.TotalMilliseconds);
                    return ValueTask.CompletedTask;
                }
            })
            .Build();
    }

    public bool IsConnected => _connection is { IsOpen: true } && !_disposed;

    public async Task<IChannel> CreateChannelAsync()
    {
        if (!IsConnected)
        {
            throw new InvalidOperationException("RabbitMQ connection is not open");
        }

        return await _connection!.CreateChannelAsync();
    }

    public async Task ConnectAsync()
    {
        await _connectionLock.WaitAsync();
        try
        {
            if (IsConnected)
            {
                return;
            }

            _logger.LogInformation("Connecting to RabbitMQ at {HostName}:{Port}", 
                _settings.HostName, _settings.Port);

            await _retryPipeline.ExecuteAsync(async cancellationToken =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                    RequestedHeartbeat = TimeSpan.FromSeconds(60),
                };

                _connection = await factory.CreateConnectionAsync(cancellationToken);
                
                _connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
                _connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
                _connection.ConnectionBlockedAsync += OnConnectionBlockedAsync;

                _logger.LogInformation("Successfully connected to RabbitMQ");
            });
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed) return;
        
        _logger.LogWarning("RabbitMQ connection blocked: {Reason}", e.Reason);
        await TryReconnectAsync();
    }

    private async Task OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
    {
        if (_disposed) return;
        
        _logger.LogWarning(e.Exception, "RabbitMQ callback exception");
        await TryReconnectAsync();
    }

    private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs e)
    {
        if (_disposed) return;
        
        _logger.LogWarning("RabbitMQ connection shutdown: {ReplyText}", e.ReplyText);
        await TryReconnectAsync();
    }

    private async Task TryReconnectAsync()
    {
        if (_disposed) return;

        try
        {
            await ConnectAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to reconnect to RabbitMQ");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;

        try
        {
            if (_connection != null)
            {
                _connection.ConnectionShutdownAsync -= OnConnectionShutdownAsync;
                _connection.CallbackExceptionAsync -= OnCallbackExceptionAsync;
                _connection.ConnectionBlockedAsync -= OnConnectionBlockedAsync;
                
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ connection");
        }
        finally
        {
            _connectionLock.Dispose();
        }
    }
}
