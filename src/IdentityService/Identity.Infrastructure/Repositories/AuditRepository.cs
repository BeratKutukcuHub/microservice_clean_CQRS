using AbstractionBlocks.Common.Infrastructure;
using AbstractionBlocks.Common.Infrastructure.Persistance;
using AbstractionBlocks.Common.Pagination;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using IdentityService.Identity.Infrastructure.Exceptions;
using IdentityService.Identity.Infrastructure.Extensions;
using MongoDB.Driver;
namespace IdentityService.Identity.Infrastructure.Repositories
{
    public class AuditRepository : Repository<AuditLog>, IAuditRepository
    {
        public AuditRepository(MongoDatabase<AuditLog> collection) : base(collection)
        {
        }
        public async Task<bool> AddAuditLogAsync(AuditLog auditLog)
        {
            try
            {
                await _collection.Collection.InsertOneAsync(auditLog);
                return true;
            }
            catch (MongoWriteException ex)
            {
                throw new DatabaseOperationException("Failed to add audit log", ex);
            }
        }
        public async Task<bool> AddAuditLogsAsync(List<AuditLog> auditLogs)
        {
            try
            {
                await _collection.Collection.InsertManyAsync(auditLogs);
                return true;
            }
            catch (MongoWriteException ex)
            {
                throw new DatabaseOperationException("Failed to add audit logs", ex);
            }
        }
        public async Task<List<AuditLog>> GetAuditLogAsync(List<Guid> Id)
        {
            try
            {
                var result = await _collection.Collection.Find(x => Id.Contains(x.Id)).ToListAsync();
                return result;
            }
            catch (MongoWriteException ex)
            {
                throw new DatabaseOperationException("Failed to get audit logs", ex);
            }
        }
        public async Task<PaginationResponse<AuditLog>> GetAuditLogsAsync(PaginationValue pag)
        {
            try
            {
                var result = await _collection.Collection.GetAllPaginationAsync(pag);
                return result;
            }
            catch (MongoWriteException ex)
            {
                throw new DatabaseOperationException("Failed to get audit logs", ex);
            }
        }
    }
}