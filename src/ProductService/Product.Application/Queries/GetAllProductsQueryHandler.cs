using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Pagination;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductEntity = ProductService.Product.Domain.Product;
namespace ProductService.Product.Application.Queries
{
    [Cache("ProductsPaginated", 5)]
    public record GetAllProductsQuery(int PageNumber, int PageSize) : IRequest<PaginationResponse<ProductEntity>?>;
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginationResponse<ProductEntity>?>
    {
        private readonly IUnitOfWork _uow;
        public GetAllProductsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<PaginationResponse<ProductEntity>?> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            var paginationValue = new PaginationValue
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
            return await _uow.ProductRepository.GetAllPagination(paginationValue);
        }
    }
}
