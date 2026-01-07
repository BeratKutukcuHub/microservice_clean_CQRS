using AbstractionBlocks.Common.Application.Interfaces;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
namespace AbstractionBlocks.Common.Infrastructure.UnitOfWork;
public class MongoUnitOfWork : IUnitOfWork
{
    private readonly ILogger<MongoUnitOfWork> _logger;
    private IClientSessionHandle? _session;
    private readonly IMongoClient _mongoClient;
    public MongoUnitOfWork(IMongoClient mongoClient, ILogger<MongoUnitOfWork> logger)
    {
        _mongoClient = mongoClient;
        _logger = logger;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("SaveChanges called (MongoDB auto-saves)");
        return await Task.FromResult(1);
    }
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }
        _session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken);
        _session.StartTransaction();
        _logger.LogInformation("MongoDB transaction started");
    }
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session == null)
        {
            throw new InvalidOperationException("No transaction to commit");
        }
        try
        {
            await _session.CommitTransactionAsync(cancellationToken);
            _logger.LogInformation("MongoDB transaction committed successfully");
        }
        catch (global::System.Exception ex)
        {
            _logger.LogError(ex, "Error occurred while committing MongoDB transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _session.Dispose();
            _session = null;
        }
    }
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_session == null)
        {
            throw new InvalidOperationException("No transaction to rollback");
        }
        try
        {
            await _session.AbortTransactionAsync(cancellationToken);
            _logger.LogInformation("MongoDB transaction rolled back");
        }
        catch (global::System.Exception ex)
        {
            _logger.LogError(ex, "Error occurred while rolling back MongoDB transaction");
            throw;
        }
        finally
        {
            _session.Dispose();
            _session = null;
        }
    }
    public void Dispose()
    {
        _session?.Dispose();
    }
}
