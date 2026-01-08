using MediatR;
using ProductService.Product.Application.DTOs;
using ProductService.Product.Application.UOW;

namespace ProductService.Product.Application.Queries
{
    public record SearchProductsQuery(string SearchTerm) : IRequest<List<ProductDto>>;

    public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
    {
        private readonly IUnitOfWork _uow;

        public SearchProductsQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
        {
            var products = await _uow.ProductRepository.GetAllAsync();
            
            var searchTerm = request.SearchTerm.ToLower();
            var filteredProducts = products
                .Where(p => !p.IsDeleted && 
                           p.IsActive && 
                           (p.Name.ToLower().Contains(searchTerm) || 
                            p.Description.ToLower().Contains(searchTerm) ||
                            (p.Category != null && p.Category.ToLower().Contains(searchTerm))))
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
