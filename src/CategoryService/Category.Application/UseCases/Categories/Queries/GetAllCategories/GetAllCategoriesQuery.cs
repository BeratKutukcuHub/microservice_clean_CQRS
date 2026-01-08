using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Pagination;
using MediatR;

namespace Category.Application.UseCases.Categories.Queries.GetAllCategories;

[Cache("all-categories", 10)]
public record GetAllCategoriesQuery(PaginationValue Pagination) : IRequest<PaginationResponse<CategoryListDto>>;
