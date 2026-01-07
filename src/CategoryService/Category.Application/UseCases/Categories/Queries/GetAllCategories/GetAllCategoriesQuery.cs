using AbstractionBlocks.Common.Pagination;
using MediatR;
namespace Category.Application.UseCases.Categories.Queries.GetAllCategories;
public record GetAllCategoriesQuery(PaginationValue Pagination) : IRequest<PaginationResponse<CategoryListDto>>;
