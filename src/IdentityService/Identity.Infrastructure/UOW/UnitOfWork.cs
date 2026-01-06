using IdentityService.Application.Helper;
using IdentityService.Application.Interfaces;
using IdentityService.Application.UOW;
using IdentityService.Identity.Application.Provider;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MongoDB.Driver;

namespace IdentityService.Identity.Infrastructure.UOW
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICurrentUser CurrentUser { get; }
        public IIdentityRepository IdentityRepository { get; }
        public IRoleRepository RoleRepository { get; }

        public IAuditRepository AuditRepository { get; }

        public IJwtTokenGenerator JwtTokenGenerator { get; }

        private readonly IMongoClient _client;
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
        public async Task<IdentityUserPermissions?> IdentityUserAssingRoleAsync(Guid userId, Guid roleId, Guid oldRoleId)
        {
            IdentityUser identity = null;
            Role role = null;
            await TransactionAsync(async x =>
            {
                role = await ((Repositories.RoleRepository)RoleRepository).GetByIdSessionAsync(x, roleId);
                identity = await ((Repositories.IdentityUserRepository)IdentityRepository).GetByIdSessionAsync(x, userId,
                roleId,
                oldRoleId);
            });
            return IdentityUserPermissions.Create(identity, role.Permissions);
        }

    }
}
