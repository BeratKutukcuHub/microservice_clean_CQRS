using AbstractionBlocks.Common.Domain;
using Category.Domain.Events;
namespace Category.Domain;
public class Category : Entity, IAggregateRoot
{
    private readonly List<IEventDomain> _events = new();
    public string Description { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? ParentCategoryId { get; private set; }
    public IReadOnlyList<IEventDomain> Events => _events.AsReadOnly();
    private Category() { } 
    private Category(string name, string description, string? imageUrl, Guid? parentCategoryId, Guid createdById)
    {
        ValidateName(name);
        ValidateDescription(description);
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
        ParentCategoryId = parentCategoryId;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        CreateById = createdById;
    }
    public static Category Create(string name, string description, string? imageUrl = null, Guid? parentCategoryId = null, Guid? createdById = null)
    {
        var category = new Category(name, description, imageUrl, parentCategoryId, createdById ?? Guid.Empty);
        
        // Raise domain event
        category.AddEvent(new CategoryCreatedEvent(category.Id, category.Name, category.Description));
        
        // Raise integration event
        category.RaiseCategoryCreatedIntegrationEvent(createdById ?? Guid.Empty);
        
        return category;
    }

    public void Update(string name, string? description, bool isActive, Guid updatedBy)
    {
        ValidateName(name);
        if (!string.IsNullOrWhiteSpace(description))
        {
            ValidateDescription(description);
        }
        Name = name;
        Description = description ?? string.Empty;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
        
        // Raise domain event
        AddEvent(new CategoryUpdatedEvent(Id, Name, Description));
        
        // Raise integration event
        RaiseCategoryUpdatedIntegrationEvent(updatedBy);
    }
    public void SetParentCategory(Guid? parentCategoryId, Guid updatedById)
    {
        if (parentCategoryId == Id)
            throw new InvalidOperationException("Category cannot be its own parent");
        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
        UpdatedById = updatedById;
    }
    public void Activate(Guid updatedById)
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
        UpdatedById = updatedById;
    }
    public void Deactivate(Guid updatedById)
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
        UpdatedById = updatedById;
    }
    public void Delete(Guid deletedBy)
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
        
        // Raise domain event
        AddEvent(new CategoryDeletedEvent(Id, Name));
        
        // Raise integration event
        RaiseCategoryDeletedIntegrationEvent(deletedBy);
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddEvent(IEventDomain @event)
    {
        _events.Add(@event);
    }

    public void ClearEvents()
    {
        _events.Clear();
    }

    private void RaiseCategoryCreatedIntegrationEvent(Guid createdBy)
    {
        var integrationEvent = new CategoryCreatedIntegrationEvent(
            Id,
            Name,
            Description,
            ImageUrl,
            ParentCategoryId,
            createdBy);
        _events.Add(integrationEvent);
    }

    private void RaiseCategoryUpdatedIntegrationEvent(Guid updatedBy)
    {
        var integrationEvent = new CategoryUpdatedIntegrationEvent(
            Id,
            Name,
            Description,
            IsActive,
            updatedBy);
        _events.Add(integrationEvent);
    }

    private void RaiseCategoryDeletedIntegrationEvent(Guid deletedBy)
    {
        var integrationEvent = new CategoryDeletedIntegrationEvent(
            Id,
            Name,
            deletedBy);
        _events.Add(integrationEvent);
    }
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));
        if (name.Length > 100)
            throw new ArgumentException("Category name cannot exceed 100 characters", nameof(name));
    }
    private static void ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Category description cannot be empty", nameof(description));
        if (description.Length > 500)
            throw new ArgumentException("Category description cannot exceed 500 characters", nameof(description));
    }
}