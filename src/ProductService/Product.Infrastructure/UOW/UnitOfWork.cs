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
