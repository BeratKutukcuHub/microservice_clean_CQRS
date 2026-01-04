using IdentityService.Application.Exceptions;
using IdentityService.Application.Interfaces;
using IdentityService.Identity.Domain.Events;

namespace IdentityService.Application.Auth.Identity.Events
{
    public class AuditLogEventHandler : IAuditLogEventHandler
    {
        private readonly IAuditRepository _auditRepository;

        public AuditLogEventHandler(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }

        public async Task Handle(AuditLogEventDomain reqEvent)
        {
            var result = await _auditRepository.AddAuditLogAsync(reqEvent.auditLog);
            if(!result) throw new AuditNotAddException($"Failed to add audit log ID : {reqEvent.auditLog.CorrelationId}");
        }
    }
}