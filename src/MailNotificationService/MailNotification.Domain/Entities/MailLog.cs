using AbstractionBlocks.Common.Domain;
using MailNotification.Domain.Events;
namespace MailNotification.Domain.Entities
{
    public class MailLog : Entity, IAggregateRoot
    {
        private readonly List<IEventDomain> _events = new();
        public string To { get; private set; } = string.Empty;
        public string Subject { get; private set; } = string.Empty;
        public string Body { get; private set; } = string.Empty;
        public bool IsSent { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime SentAt { get; private set; }
        public IReadOnlyList<IEventDomain> Events => _events.AsReadOnly();
        private MailLog() { }
        public static MailLog Create(string to, string subject, string body)
        {
            var mailLog = new MailLog
            {
                To = to,
                Subject = subject,
                Body = body,
                IsSent = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            mailLog.AddEvent(new MailSentEvent(mailLog.Id, to, subject));
            return mailLog;
        }
        public void MarkAsSent()
        {
            IsSent = true;
            UpdatedAt = DateTime.UtcNow;
            
            // Raise integration event
            AddEvent(new MailSentIntegrationEvent(
                Id,
                To,
                Subject,
                DateTime.UtcNow));
        }
        public void MarkAsFailed(string errorMessage)
        {
            IsSent = false;
            ErrorMessage = errorMessage;
            UpdatedAt = DateTime.UtcNow;
            
            // Raise integration event
            AddEvent(new MailFailedIntegrationEvent(
                Id,
                To,
                Subject,
                errorMessage,
                DateTime.UtcNow));
        }
        public void AddEvent(IEventDomain @event)
        {
            _events.Add(@event);
        }
        public void ClearEvents()
        {
            _events.Clear();
        }
    }
}
