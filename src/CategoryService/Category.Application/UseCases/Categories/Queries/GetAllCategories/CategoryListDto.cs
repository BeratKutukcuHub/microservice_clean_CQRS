namespace Category.Application.UseCases.Categories.Queries.GetAllCategories;
public record CategoryListDto(
    Guid Id,
    string Name,
    string Description,
    bool IsActive,
    Guid? ParentCategoryId
);
