using AbstractionBlocks.Common.Domain;
using IdentityService.Identity.Domain.Events;

namespace IdentityService.Identity.Domain
{
    public class AuditLog : Entity, IAggregateRoot
    {
    private List<IEventDomain> _events = new List<IEventDomain>();
    public IReadOnlyList<IEventDomain> Events => _events;
    public string EntityName { get; private set; }        
    public Guid EntityId { get; private set; }          
    public string Action { get; private set; }            
    public Guid UserId { get; private set; }            
    public DateTime Timestamp { get; private set; }       
    public List<ChangeDetail> Changes { get; private set; }  // JSON veya key-value değişiklikler
    public Guid CorrelationId { get; private set; }     
    public string Source { get; private set; }              // Web, API, Background job

        

        private AuditLog(
            string entityName, Guid entityId, string action, Guid userId, DateTime timestamp, Guid correlationId, string source
, List<ChangeDetail> changes)
        {
            CreatedAt = timestamp;
            Id = Guid.NewGuid();
            EntityName = entityName;
            EntityId = entityId;
            Action = action;
            UserId = userId;
            Timestamp = timestamp;
            CorrelationId = correlationId;
            Source = source;
            Changes = changes;
        }
        public static AuditLog Create(
            string entityName, Guid entityId, string action, Guid userId, Guid correlationId, string source, List<ChangeDetail> changes
        ) => new(entityName, entityId, action, userId, DateTime.UtcNow, correlationId, source, changes);
        public void AddChange(ChangeDetail changeDetail) => Changes.Add(changeDetail);
        public void AddAuditEvent() => _events.Add(new AuditLogEventDomain(this));
    }
}