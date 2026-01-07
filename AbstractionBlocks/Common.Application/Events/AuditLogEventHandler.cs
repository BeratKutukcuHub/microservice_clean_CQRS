using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Exception.Logger;
using System.Threading.Tasks;
namespace AbstractionBlocks.Common.Application.Events
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
            {
            }
        }
    }
}
