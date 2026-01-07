using MediatR;
namespace Category.Application.UseCases.Categories.Commands.UpdateCategory;
public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<UpdateCategoryResponse>;
