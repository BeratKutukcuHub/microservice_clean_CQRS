namespace Category.Application.UseCases.Categories.Commands.CreateCategory;
public record CreateCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    Guid? ParentCategoryId,
    DateTime CreatedAt
);
