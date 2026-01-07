using AbstractionBlocks.Common.Application.Interfaces;
using ProductService.Product.Application.Repository;
using MongoDB.Driver;
namespace ProductService.Product.Infrastructure.UOW
{
    public class UnitOfWork : ProductService.Product.Application.UOW.IUnitOfWork
    {
        public ICurrentUser CurrentUser { get; }
        public IProductRepository ProductRepository { get; }
        public IAuditRepository AuditRepository { get; }
        private readonly IMongoClient _client;
        private IClientSessionHandle? _session;

        public UnitOfWork(
            IProductRepository productRepository,
            IAuditRepository auditRepository,
            IMongoClient client,
            ICurrentUser currentUser)
        {
            ProductRepository = productRepository;
            AuditRepository = auditRepository;
            _client = client;
            CurrentUser = currentUser;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // MongoDB doesn't have a traditional SaveChanges concept
            // Changes are saved immediately with each operation
            return await Task.FromResult(0);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
            _session.StartTransaction();
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                await _session.CommitTransactionAsync(cancellationToken);
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                await _session.AbortTransactionAsync(cancellationToken);
            }
        }

        public void Dispose()
        {
            _session?.Dispose();
        }

        private async Task TransactionAsync(Func<IClientSessionHandle, Task> action)
        {
            using var session = await _client.StartSessionAsync();
            session.StartTransaction();
            try
            {
                await action.Invoke(session);
                await session.CommitTransactionAsync();
            }
            catch
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }
    }
}
