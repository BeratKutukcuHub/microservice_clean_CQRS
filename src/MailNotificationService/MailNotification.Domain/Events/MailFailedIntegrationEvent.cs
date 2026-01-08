using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Messaging.Events;

namespace MailNotification.Domain.Events;

public class MailFailedIntegrationEvent : IntegrationEvent, IEventDomain
{
    public Guid MailLogId { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime FailedAt { get; set; }

    public MailFailedIntegrationEvent(
        Guid mailLogId,
        string to,
        string subject,
        string errorMessage,
        DateTime failedAt) : base("MailNotificationService", "v1")
    {
        MailLogId = mailLogId;
        To = to;
        Subject = subject;
        ErrorMessage = errorMessage;
        FailedAt = failedAt;
    }
}
