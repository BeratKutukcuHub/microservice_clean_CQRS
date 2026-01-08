using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Domain;

namespace Category.Domain.Events;

public class CategoryCreatedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Guid CreatedBy { get; set; }

    public CategoryCreatedIntegrationEvent(
        Guid categoryId,
        string name,
        string description,
        string? imageUrl,
        Guid? parentCategoryId,
        Guid createdBy) : base("CategoryService", "v1")
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        ParentCategoryId = parentCategoryId;
        CreatedBy = createdBy;
    }
}
