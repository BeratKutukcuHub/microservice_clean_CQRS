namespace MailNotification.Application.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    }
}
