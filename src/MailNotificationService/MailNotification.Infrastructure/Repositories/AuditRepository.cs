using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Pagination;
using MongoDB.Driver;
namespace MailNotification.Infrastructure.Repositories;
public class AuditRepository : IAuditRepository
{
    private readonly IMongoCollection<AuditLog> _collection;
    public AuditRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<AuditLog>("AuditLogs");
    }
    public async Task<bool> AddAuditLogAsync(AuditLog auditLog)
    {
        try
        {
            await _collection.InsertOneAsync(auditLog);
            return true;
        }
        catch (MongoWriteException ex)
        {
            throw new Exception("Failed to add audit log", ex);
        }
    }
    public async Task<bool> AddAuditLogsAsync(List<AuditLog> auditLogs)
    {
        try
        {
            await _collection.InsertManyAsync(auditLogs);
            return true;
        }
        catch (MongoWriteException ex)
        {
            throw new Exception("Failed to add audit logs", ex);
        }
    }
    public async Task<List<AuditLog>> GetAuditLogAsync(List<Guid> Id)
    {
        try
        {
            var result = await _collection.Find(x => Id.Contains(x.Id)).ToListAsync();
            return result;
        }
        catch (MongoWriteException ex)
        {
            throw new Exception("Failed to get audit logs", ex);
        }
    }
    public async Task<PaginationResponse<AuditLog>> GetAuditLogsAsync(PaginationValue pag)
    {
        try
        {
            var totalCount = await _collection.CountDocumentsAsync(FilterDefinition<AuditLog>.Empty);
            var items = await _collection.Find(FilterDefinition<AuditLog>.Empty)
                .Skip((pag.PageNumber - 1) * pag.PageSize)
                .Limit(pag.PageSize)
                .ToListAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pag.PageSize);
            return PaginationResponse<AuditLog>.Create(
                pag.PageNumber,
                pag.PageSize,
                (int)totalCount,
                totalPages,
                items
            );
        }
        catch (MongoWriteException ex)
        {
            throw new Exception("Failed to get audit logs", ex);
        }
    }
}
