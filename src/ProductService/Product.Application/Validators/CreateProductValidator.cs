using FluentValidation;
using ProductService.Product.Application.Commands;
namespace ProductService.Product.Application.Validators
{
    public class CreateProductValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.");
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Product price must be greater than or equal to 0.");
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Product stock must be greater than or equal to 0.");
            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Category));
        }
    }
}
