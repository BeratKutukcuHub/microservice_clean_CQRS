namespace Category.Application.UseCases.Categories.Commands.UpdateCategory;
public record UpdateCategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime UpdatedAt
);
