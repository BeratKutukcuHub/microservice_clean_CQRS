using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace MailNotification.Domain.Events;

public class MailSentIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid MailLogId { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public DateTime SentAt { get; set; }

    public MailSentIntegrationEvent(
        Guid mailLogId,
        string to,
        string subject,
        DateTime sentAt) : base("MailNotificationService", "v1")
    {
        MailLogId = mailLogId;
        To = to;
        Subject = subject;
        SentAt = sentAt;
    }
}
