using FluentValidation;
namespace Category.Application.UseCases.Categories.Commands.CreateCategory;
public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Category description is required")
            .MaximumLength(500).WithMessage("Category description cannot exceed 500 characters");
        RuleFor(x => x.ImageUrl)
            .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.ImageUrl))
            .WithMessage("Image URL must be a valid URL");
        RuleFor(x => x.ParentCategoryId)
            .NotEqual(Guid.Empty).When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Parent category ID must be a valid GUID");
    }
    private static bool BeValidUrl(string? url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}