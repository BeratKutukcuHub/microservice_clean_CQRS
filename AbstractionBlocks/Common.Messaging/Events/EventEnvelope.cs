namespace AbstractionBlocks.Common.Messaging.Events;

public class EventEnvelope<TEvent> where TEvent : IntegrationEvent
{
    public Guid MessageId { get; set; }
    public string MessageType { get; set; }
    public DateTime Timestamp { get; set; }
    public TEvent Payload { get; set; }
    public Dictionary<string, string> Headers { get; set; }

    public EventEnvelope(TEvent payload)
    {
        MessageId = Guid.NewGuid();
        MessageType = typeof(TEvent).Name;
        Timestamp = DateTime.UtcNow;
        Payload = payload;
        Headers = new Dictionary<string, string>();
    }
}
