using AbstractionBlocks.Common.Domain;
namespace Category.Domain.Events;
public class CategoryCreatedEvent : IEventDomain
{
    public Guid CategoryId { get; }
    public string Name { get; }
    public string Description { get; }
    public CategoryCreatedEvent(Guid categoryId, string name, string description)
    {
        CategoryId = categoryId;
        Name = name;
        Description = description;
    }
}