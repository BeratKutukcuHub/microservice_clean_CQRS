using AbstractionBlocks.Common.SecretBase.Options;
using AbstractionBlocks.Common.SecretBase.Provider;
using MailKit.Net.Smtp;
using MailNotification.Application.Interfaces;
using MimeKit;
namespace MailNotification.Infrastructure.Services
{
    public class MailService : IMailService
    {
        private readonly SmtpOptions _smtpOptions;
        public MailService(ISecretProvider<SmtpOptions> secretProvider)
        {
            _smtpOptions = secretProvider.GetSection();
        }
        public async Task<bool> SendMailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();
                using var client = new SmtpClient();
                await client.ConnectAsync(_smtpOptions.SmtpServer, _smtpOptions.Port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await client.AuthenticateAsync(_smtpOptions.Username, _smtpOptions.Password, cancellationToken);
                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
