using AbstractionBlocks.Common.Messaging.Events;

namespace AbstractionBlocks.Common.Messaging.Interfaces;

public interface IEventHandler<in TEvent> where TEvent : IntegrationEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}
