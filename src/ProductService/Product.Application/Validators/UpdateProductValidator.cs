using FluentValidation;
using ProductService.Product.Application.Commands;
namespace ProductService.Product.Application.Validators
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Product ID is required.");
            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Name));
            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Product description cannot exceed 1000 characters.")
                .When(x => x.Description != null);
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Product price must be greater than or equal to 0.")
                .When(x => x.Price.HasValue);
            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Product stock must be greater than or equal to 0.")
                .When(x => x.Stock.HasValue);
            RuleFor(x => x.Category)
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.Category));
        }
    }
}
