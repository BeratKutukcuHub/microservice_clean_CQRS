namespace Shared.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync(string exchange, string routingKey, string message);
    }
}