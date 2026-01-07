using AbstractionBlocks.Common.Domain;
namespace Category.Domain.Events;
public record CategoryDeletedEvent(Guid CategoryId, string CategoryName) : IEventDomain;
