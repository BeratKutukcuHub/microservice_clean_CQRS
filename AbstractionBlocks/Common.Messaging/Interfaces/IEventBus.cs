using AbstractionBlocks.Common.Messaging.Events;

namespace AbstractionBlocks.Common.Messaging.Interfaces;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IntegrationEvent;
    
    Task PublishAsync<TEvent>(TEvent @event, string routingKey, CancellationToken cancellationToken = default) 
        where TEvent : IntegrationEvent;
    
    void Subscribe<TEvent, THandler>() 
        where TEvent : IntegrationEvent 
        where THandler : IEventHandler<TEvent>;
    
    void Unsubscribe<TEvent, THandler>() 
        where TEvent : IntegrationEvent 
        where THandler : IEventHandler<TEvent>;
}
