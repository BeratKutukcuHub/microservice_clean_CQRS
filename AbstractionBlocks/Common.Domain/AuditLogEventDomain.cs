namespace AbstractionBlocks.Common.Domain
{
    public record AuditLogEventDomain(AuditLog auditLog) : IEventDomain;
}
