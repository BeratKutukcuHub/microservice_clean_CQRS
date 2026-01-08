using FluentValidation;

namespace MailNotification.Application.Commands.Validators;

public class SendMailCommandValidator : AbstractValidator<SendMailCommand>
{
    public SendMailCommandValidator()
    {
        RuleFor(x => x.To)
            .NotEmpty().WithMessage("Recipient email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(200).WithMessage("Email cannot exceed 200 characters");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required")
            .MaximumLength(500).WithMessage("Subject cannot exceed 500 characters");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required")
            .MaximumLength(10000).WithMessage("Body cannot exceed 10000 characters");
    }
}
