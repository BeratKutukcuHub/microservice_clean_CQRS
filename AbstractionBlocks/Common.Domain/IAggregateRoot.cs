namespace AbstractionBlocks.Common.Domain
{
    public interface IAggregateRoot : IEventDomain
    {
        IReadOnlyList<IEventDomain> Events { get; }
    }
} 
