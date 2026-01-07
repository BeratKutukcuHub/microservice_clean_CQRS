using MediatR;
namespace Category.Application.UseCases.Categories.Queries.GetCategoryById;
public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
