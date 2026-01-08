using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Domain;

namespace ProductService.Product.Domain.Events;

public class ProductUpdatedEvent : IntegrationEvent, IEventDomain
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Category { get; set; }
    public Guid UpdatedBy { get; set; }

    public ProductUpdatedEvent(
        Guid productId,
        string name,
        string? description,
        decimal price,
        int stock,
        string? category,
        Guid updatedBy) : base("ProductService", "v1")
    {
        ProductId = productId;
        Name = name;
        Description = description;
        Price = price;
        Stock = stock;
        Category = category;
        UpdatedBy = updatedBy;
    }
}
