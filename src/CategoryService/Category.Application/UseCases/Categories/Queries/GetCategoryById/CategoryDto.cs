namespace Category.Application.UseCases.Categories.Queries.GetCategoryById;
public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    string? ImageUrl,
    bool IsActive,
    Guid? ParentCategoryId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
