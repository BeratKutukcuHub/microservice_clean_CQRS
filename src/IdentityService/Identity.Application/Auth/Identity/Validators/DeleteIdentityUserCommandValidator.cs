using FluentValidation;
using IdentityService.Application.Auth.Identity.Commands;
namespace IdentityService.Application.Auth.Identity.Validators
{
    public class DeleteIdentityUserCommandValidator : AbstractValidator<DeleteIdentityUserCommand>
    {
        public DeleteIdentityUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required.");
        }
    }
}
