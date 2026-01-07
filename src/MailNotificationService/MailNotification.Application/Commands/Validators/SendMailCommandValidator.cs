using FluentValidation;
namespace MailNotification.Application.Commands.Validators
{
    public class SendMailCommandValidator : AbstractValidator<SendMailCommand>
    {
        public SendMailCommandValidator()
        {
            ClassLevelCascadeMode = CascadeMode.Continue;
            RuleFor(x => x.To)
                .NotEmpty().WithMessage("Recipient email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(200).WithMessage("Subject must not exceed 200 characters.");
            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required.");
        }
    }
}
