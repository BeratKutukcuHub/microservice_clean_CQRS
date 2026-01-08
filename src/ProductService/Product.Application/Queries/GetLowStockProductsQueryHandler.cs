using AbstractionBlocks.Common.Application.Caching;
using MediatR;
using ProductService.Product.Application.DTOs;
using ProductService.Product.Application.UOW;

namespace ProductService.Product.Application.Queries
{
    [Cache("low-stock-products", 5)]
    public record GetLowStockProductsQuery(int Threshold = 10) : IRequest<List<ProductDto>>;

    public class GetLowStockProductsQueryHandler : IRequestHandler<GetLowStockProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetLowStockProductsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ProductDto>> Handle(GetLowStockProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _uow.ProductRepository.GetAllAsync();
            
            var lowStockProducts = products
                .Where(p => !p.IsDeleted && p.IsActive && p.Stock <= request.Threshold)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Stock = p.Stock,
                    Category = p.Category,
                    IsActive = p.IsActive
                })
                .OrderBy(p => p.Stock)
                .ToList();

            return lowStockProducts;
        }
    }
}
