using AbstractionBlocks.Common.Messaging.Events;
using AbstractionBlocks.Common.Domain;

namespace ProductService.Product.Domain.Events;

public class ProductStockChangedEvent : IntegrationEvent, IEventDomain
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public int OldStock { get; set; }
    public int NewStock { get; set; }
    public string Reason { get; set; }
    public Guid ChangedBy { get; set; }

    public ProductStockChangedEvent(
        Guid productId,
        string name,
        int oldStock,
        int newStock,
        string reason,
        Guid changedBy) : base("ProductService", "v1")
    {
        ProductId = productId;
        Name = name;
        OldStock = oldStock;
        NewStock = newStock;
        Reason = reason;
        ChangedBy = changedBy;
    }
}
