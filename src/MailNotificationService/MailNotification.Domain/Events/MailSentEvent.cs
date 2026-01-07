using AbstractionBlocks.Common.Domain;
namespace MailNotification.Domain.Events
{
    public class MailSentEvent : IEventDomain
    {
        public Guid MailLogId { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public MailSentEvent(Guid mailLogId, string to, string subject)
        {
            MailLogId = mailLogId;
            To = to;
            Subject = subject;
        }
    }
}
