using AbstractionBlocks.Common.Pagination;
using Category.Application.Interfaces;
using MediatR;
namespace Category.Application.UseCases.Categories.Queries.GetAllCategories;
public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, PaginationResponse<CategoryListDto>>
{
    private readonly ICategoryRepository _repository;
    public GetAllCategoriesHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }
    public async Task<PaginationResponse<CategoryListDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var result = await _repository.GetAllPaginationAsync(request.Pagination);
        var dtos = result.Data.Select(c => new CategoryListDto(
            c.Id,
            c.Name!,
            c.Description,
            c.IsActive,
            c.ParentCategoryId
        )).ToList();
        return PaginationResponse<CategoryListDto>.Create(
            result.PageNumber,
            result.PageSize,
            result.TotalCount,
            result.TotalPages,
            dtos
        );
    }
}
