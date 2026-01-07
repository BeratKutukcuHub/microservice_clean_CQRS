using AbstractionBlocks.Common.Application.Repository;
using AbstractionBlocks.Common.Pagination;
using ProductService.Product.Domain;
using ProductEntity = ProductService.Product.Domain.Product;
namespace ProductService.Product.Application.Repository
{
    public interface IProductRepository : IRepository<ProductEntity>
    {
        Task<PaginationResponse<ProductEntity>?> GetAllPagination(PaginationValue paginationValue);
        Task<IEnumerable<ProductEntity>> GetActiveProductsAsync();
    }
}
