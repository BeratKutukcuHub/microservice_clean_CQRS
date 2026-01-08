using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Domain;

namespace Category.Domain.Events;

public class CategoryUpdatedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public Guid UpdatedBy { get; set; }

    public CategoryUpdatedIntegrationEvent(
        Guid categoryId,
        string name,
        string description,
        bool isActive,
        Guid updatedBy) : base("CategoryService", "v1")
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        IsActive = isActive;
        UpdatedBy = updatedBy;
    }
}
