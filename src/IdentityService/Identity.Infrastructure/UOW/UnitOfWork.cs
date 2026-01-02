using IdentityService.Application.Interfaces;
using IdentityService.Application.UOW;
using IdentityService.Identity.Domain.Repository;
using IdentityService.Identity.Infrastructure.Repositories;
using MongoDB.Driver;

namespace IdentityService.Identity.Infrastructure.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICurrentUser CurrentUser { get; }
        public IIdentityRepository IdentityRepository { get; }
        public IRoleRepository RoleRepository { get; }
        private readonly IMongoClient _client;
        public UnitOfWork(IRoleRepository roleRepository,
        IIdentityRepository ıdentityRepository,
        IMongoClient client, ICurrentUser currentUser)
        {
            RoleRepository = roleRepository;
            IdentityRepository = ıdentityRepository;
            _client = client;
            CurrentUser = currentUser;
        }

        public async Task<bool> TransactionAsync(Func<IClientSessionHandle, Task> action)
        {
            using var session = await _client.StartSessionAsync();
            session.StartTransaction();
            try
            {
                await action.Invoke(session);
                await session.CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {
                await session.AbortTransactionAsync();
                throw;
            }
        }
    }
}
