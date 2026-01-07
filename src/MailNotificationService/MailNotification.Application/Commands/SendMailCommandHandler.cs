using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Exception.Logger;
using MailNotification.Application.Interfaces;
using MailNotification.Domain.Entities;
using MailNotification.Domain.Events;
using MediatR;
namespace MailNotification.Application.Commands
{
    public class SendMailCommandHandler : IRequestHandler<SendMailCommand, Guid>
    {
        private readonly IMailService _mailService;
        private readonly ILoggerService<SendMailCommandHandler> _logger;
        private readonly ICurrentUser _currentUser;
        private readonly IApplicationDispatcher _dispatcher;
        public SendMailCommandHandler(
            IMailService mailService,
            ILoggerService<SendMailCommandHandler> logger,
            ICurrentUser currentUser,
            IApplicationDispatcher dispatcher)
        {
            _mailService = mailService;
            _logger = logger;
            _currentUser = currentUser;
            _dispatcher = dispatcher;
        }
        public async Task<Guid> Handle(SendMailCommand request, CancellationToken cancellationToken)
        {
            var mailLog = MailLog.Create(request.To, request.Subject, request.Body);
            try
            {
                var success = await _mailService.SendMailAsync(
                    request.To,
                    request.Subject,
                    request.Body,
                    cancellationToken);
                if (success)
                {
                    mailLog.MarkAsSent();
                    _logger.Information($"Mail sent successfully to {request.To}", new { request.To, request.Subject });
                    var mailSentEvent = new MailSentEvent(mailLog.Id, request.To, request.Subject);
                    mailLog.AddEvent(mailSentEvent);
                }
                else
                {
                    mailLog.MarkAsFailed("Failed to send mail");
                    _logger.Warning($"Failed to send mail to {request.To}", new { request.To, request.Subject });
                }
            }
            catch (Exception ex)
            {
                mailLog.MarkAsFailed(ex.Message);
                _logger.Error(ex, $"Error sending mail to {request.To}", new { request.To, request.Subject });
                throw;
            }
            var audit = AuditLog.Create(
                "MailLog",
                mailLog.Id,
                "SendMail",
                _currentUser.UserId,
                _currentUser.CorrelationId,
                "SendMailCommandHandler",
                null);
            audit.AddAuditEvent();
            await _dispatcher.Dispatch(audit.Events);
            return mailLog.Id;
        }
    }
}
