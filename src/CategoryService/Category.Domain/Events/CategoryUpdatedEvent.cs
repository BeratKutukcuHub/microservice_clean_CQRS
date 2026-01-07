using AbstractionBlocks.Common.Domain;
namespace Category.Domain.Events;
public class CategoryUpdatedEvent : IEventDomain
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public string Description { get; }
    public DateTime UpdatedAt { get; }
    public CategoryUpdatedEvent(Guid categoryId, string name, string description)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
}