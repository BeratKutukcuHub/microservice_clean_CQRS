namespace AbstractionBlocks.Common.Domain
{
    public interface IAggregateRoot
    {
        IReadOnlyList<IEventDomain> Events { get; }
    }
} 
