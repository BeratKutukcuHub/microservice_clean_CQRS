using FluentValidation;
using IdentityService.Application.Auth.Identity.Commands;
namespace IdentityService.Application.Auth.Identity.Validators
{
    public class CreateIdentityCommandValidator : AbstractValidator<CreateIdentityCommand>
    {
        public CreateIdentityCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.");
        }
    }
}
