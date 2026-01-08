using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Domain;

namespace ProductService.Product.Domain.Events;

public class ProductDeletedEvent : IntegrationEvent, IEventDomain
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public Guid DeletedBy { get; set; }

    public ProductDeletedEvent(
        Guid productId,
        string name,
        Guid deletedBy) : base("ProductService", "v1")
    {
        ProductId = productId;
        Name = name;
        DeletedBy = deletedBy;
    }
}
