using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Domain;

namespace Category.Domain.Events;

public class CategoryDeletedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public Guid DeletedBy { get; set; }

    public CategoryDeletedIntegrationEvent(
        Guid categoryId,
        string name,
        Guid deletedBy) : base("CategoryService", "v1")
    {
        CategoryId = categoryId;
        Name = name;
        DeletedBy = deletedBy;
    }
}
