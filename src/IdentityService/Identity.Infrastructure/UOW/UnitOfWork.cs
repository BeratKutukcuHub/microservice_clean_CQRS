using AbstractionBlocks.Common.Application.Interfaces;
using IdentityService.Application.Helper;
using IdentityService.Application.UOW;
using IdentityService.Identity.Application.Provider;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MongoDB.Driver;
namespace IdentityService.Identity.Infrastructure.UOW
{
    public class UnitOfWork : IdentityService.Application.UOW.IUnitOfWork
    {
        public ICurrentUser CurrentUser { get; }
        public IIdentityRepository IdentityRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IAuditRepository AuditRepository { get; }
        public IJwtTokenGenerator JwtTokenGenerator { get; }
        private readonly IMongoClient _client;
        private IClientSessionHandle? _session;
        public UnitOfWork(IRoleRepository roleRepository,
        IIdentityRepository ıdentityRepository,
        IMongoClient client, ICurrentUser currentUser, IAuditRepository auditRepository, IJwtTokenGenerator jwtTokenGenerator)
        {
            RoleRepository = roleRepository;
            IdentityRepository = ıdentityRepository;
            _client = client;
            CurrentUser = currentUser;
            AuditRepository = auditRepository;
            JwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(1);
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session != null)
            {
                throw new InvalidOperationException("Transaction already started");
            }
            _session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
            _session.StartTransaction();
        }
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("No transaction to commit");
            }
            await _session.CommitTransactionAsync(cancellationToken);
            _session.Dispose();
            _session = null;
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_session == null)
            {
                throw new InvalidOperationException("No transaction to rollback");
            }
            await _session.AbortTransactionAsync(cancellationToken);
            _session.Dispose();
            _session = null;
        }
        public void Dispose()
        {
            _session?.Dispose();
        }
        public async Task<IdentityUser> CreateIdentityUserAsync(IdentityUser user)
        {
            await IdentityRepository.AddAsync(user);
            return user;
        }
        public async Task<IdentityUser> UpdateIdentityUserAsync(IdentityUser user)
        {
            await IdentityRepository.UpdateAsync(user);
            return user;
        }
        public async Task<IdentityUser> DeleteIdentityUserAsync(Guid id)
        {
            var user = await IdentityRepository.GetByIdAsync(id);
            if (user != null)
            {
                await IdentityRepository.DeleteAsync(id);
            }
            return user!;
        }
        public async Task<IdentityUser> GetIdentityUserByIdAsync(Guid id)
        {
            return (await IdentityRepository.GetByIdAsync(id))!;
        }
        public async Task<List<IdentityUser>> GetAllIdentityUsersAsync()
        {
            var users = await IdentityRepository.GetAllAsync();
            return users.ToList();
        }
        public async Task<(IdentityUser User, List<string> Permissions)> IdentityUserAssingRoleAsync(Guid userId, Guid roleId, Guid oldRoleId)
        {
            IdentityUser identity = null!;
            Role role = null!;
            await TransactionAsync(async x =>
            {
                role = await ((Repositories.RoleRepository)RoleRepository).GetByIdSessionAsync(x, roleId);
                identity = await ((Repositories.IdentityUserRepository)IdentityRepository).GetByIdSessionAsync(x, userId,
                roleId,
                oldRoleId);
            });
            if (identity == null || role == null)
            {
                throw new InvalidOperationException("User or Role not found");
            }
            return (identity, role.Permissions);
        }
        public async Task<IdentityUser> BlockUserAsync(Guid userId)
        {
            var user = await IdentityRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.BlockUser();
                await IdentityRepository.UpdateAsync(user);
            }
            return user!;
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
