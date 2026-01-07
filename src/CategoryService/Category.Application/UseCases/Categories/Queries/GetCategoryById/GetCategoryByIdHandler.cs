using AbstractionBlocks.Common.Exception;
using Category.Application.Interfaces;
using MediatR;
namespace Category.Application.UseCases.Categories.Queries.GetCategoryById;
public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository _repository;
    public GetCategoryByIdHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(request.Id);
        if (category == null)
        {
            throw new NotFoundException($"Category with ID {request.Id} not found");
        }
        return new CategoryDto(
            category.Id,
            category.Name!,
            category.Description,
            category.ImageUrl,
            category.IsActive,
            category.ParentCategoryId,
            category.CreatedAt,
            category.UpdatedAt
        );
    }
}
