using MediatR;
namespace MailNotification.Application.Commands
{
    public record SendMailCommand(string To, string Subject, string Body) : IRequest<Guid>;
}
