using AbstractionBlocks.Common.Application.Caching;
using MediatR;

namespace Category.Application.UseCases.Categories.Queries.GetCategoryById;

[Cache("category-by-id", 15)]
public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
