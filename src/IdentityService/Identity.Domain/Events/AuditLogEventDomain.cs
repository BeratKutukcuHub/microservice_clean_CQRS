using AbstractionBlocks.Common.Domain;

namespace IdentityService.Identity.Domain.Events
{
    public record AuditLogEventDomain(AuditLog auditLog) : IEventDomain;
}