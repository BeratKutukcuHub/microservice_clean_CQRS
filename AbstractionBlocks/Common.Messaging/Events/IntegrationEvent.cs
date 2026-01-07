namespace AbstractionBlocks.Common.Messaging.Events;

public abstract class IntegrationEvent
{
    public Guid EventId { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public string EventType { get; private set; }
    public string EventVersion { get; private set; }
    public string Source { get; private set; }
    public Guid? CorrelationId { get; set; }
    public Guid? CausationId { get; set; }
    public Dictionary<string, string> Metadata { get; private set; }

    protected IntegrationEvent(string source, string eventVersion = "v1")
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        EventType = GetType().Name;
        EventVersion = eventVersion;
        Source = source;
        Metadata = new Dictionary<string, string>();
    }

    public void AddMetadata(string key, string value)
    {
        Metadata[key] = value;
    }
}
