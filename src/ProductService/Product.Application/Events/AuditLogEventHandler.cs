using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using ProductService.Product.Application.Exceptions;
namespace ProductService.Product.Application.Events
{
    public class AuditLogEventHandler : IEventApplicationHandler<AuditLogEventDomain>
    {
        private readonly IAuditRepository _auditRepository;
        public AuditLogEventHandler(IAuditRepository auditRepository)
        {
            _auditRepository = auditRepository;
        }
        public async Task Handle(AuditLogEventDomain reqEvent)
        {
            var result = await _auditRepository.AddAuditLogAsync(reqEvent.auditLog);
            if (!result)
                throw new AuditNotAddException($"Failed to add audit log ID: {reqEvent.auditLog.CorrelationId}");
        }
    }
}
