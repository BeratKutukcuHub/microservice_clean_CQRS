using AbstractionBlocks.Common.Application.Caching;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductEntity = ProductService.Product.Domain.Product;
namespace ProductService.Product.Application.Queries
{
    [Cache("ActiveProducts", 5)]
    public record GetActiveProductsQuery() : IRequest<IEnumerable<ProductEntity>>;
    public class GetActiveProductsQueryHandler : IRequestHandler<GetActiveProductsQuery, IEnumerable<ProductEntity>>
    {
        private readonly IUnitOfWork _uow;
        public GetActiveProductsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<IEnumerable<ProductEntity>> Handle(GetActiveProductsQuery request, CancellationToken cancellationToken)
        {
            return await _uow.ProductRepository.GetActiveProductsAsync();
        }
    }
}
