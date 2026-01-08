using AbstractionBlocks.Common.Application.Caching;
using MediatR;
using ProductService.Product.Application.DTOs;
using ProductService.Product.Application.UOW;

namespace ProductService.Product.Application.Queries
{
    [Cache("products-by-category", 15)]
    public record GetProductsByCategoryQuery(string Category) : IRequest<List<ProductDto>>;

    public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _uow;

        public GetProductsByCategoryQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var products = await _uow.ProductRepository.GetAllAsync();
            
            var filteredProducts = products
                .Where(p => !p.IsDeleted && 
                           p.IsActive && 
                           p.Category != null && 
                           p.Category.Equals(request.Category, StringComparison.OrdinalIgnoreCase))
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
                .ToList();

            return filteredProducts;
        }
    }
}
