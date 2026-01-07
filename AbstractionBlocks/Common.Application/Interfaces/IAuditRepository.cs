using AbstractionBlocks.Common.Pagination;
using AbstractionBlocks.Common.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace AbstractionBlocks.Common.Application.Interfaces
{
    public interface IAuditRepository
    {
        Task<bool> AddAuditLogAsync(AuditLog auditLog);
        Task<bool> AddAuditLogsAsync(List<AuditLog> auditLogs);
        Task<PaginationResponse<AuditLog>> GetAuditLogsAsync(PaginationValue pag);
        Task<List<AuditLog>> GetAuditLogAsync(List<Guid> Id);
    }
}
