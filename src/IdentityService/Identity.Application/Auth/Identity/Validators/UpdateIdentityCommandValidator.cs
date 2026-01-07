using FluentValidation;
using IdentityService.Application.Auth.Identity.Commands;
namespace IdentityService.Application.Auth.Identity.Validators
{
    public class UpdateIdentityCommandValidator : AbstractValidator<UpdateIdentityCommand>
    {
        public UpdateIdentityCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("A valid email is required.")
                .When(x => !string.IsNullOrEmpty(x.Email));
            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .When(x => !string.IsNullOrEmpty(x.Password));
        }
    }
}
