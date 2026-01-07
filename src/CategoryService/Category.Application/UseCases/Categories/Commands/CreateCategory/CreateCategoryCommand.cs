using MediatR;
namespace Category.Application.UseCases.Categories.Commands.CreateCategory;
public record CreateCategoryCommand(
    string Name,
    string Description,
    string? ImageUrl = null,
    Guid? ParentCategoryId = null
) : IRequest<CreateCategoryResponse>;
