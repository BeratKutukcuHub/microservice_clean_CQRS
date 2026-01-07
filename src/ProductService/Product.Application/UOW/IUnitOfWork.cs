using ProductService.Product.Application.Repository;
using AbstractionBlocks.Common.Application.Interfaces;
namespace ProductService.Product.Application.UOW
{
    public interface IUnitOfWork
    {
        ICurrentUser CurrentUser { get; }
        IProductRepository ProductRepository { get; }
        IAuditRepository AuditRepository { get; }
    }
}
