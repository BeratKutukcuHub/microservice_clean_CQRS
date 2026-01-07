using AbstractionBlocks.Common.Application.Caching;
using MediatR;
using ProductService.Product.Application.UOW;
using ProductService.Product.Application.Exceptions;
using ProductEntity = ProductService.Product.Domain.Product;
namespace ProductService.Product.Application.Queries
{
    [Cache("Product", 10)]
    public record GetProductByIdQuery(Guid Id) : IRequest<ProductEntity>;
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductEntity>
    {
        private readonly IUnitOfWork _uow;
        public GetProductByIdQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<ProductEntity> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _uow.ProductRepository.GetByIdAsync(request.Id);
            if (product == null)
                throw new ProductNotFoundException(request.Id);
            return product;
        }
    }
}
