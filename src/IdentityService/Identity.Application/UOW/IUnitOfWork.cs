using IdentityService.Application.Helper;
using IdentityService.Application.Interfaces;
using IdentityService.Identity.Application.Provider;
using IdentityService.Identity.Application.Repository;

namespace IdentityService.Application.UOW
{
    public interface IUnitOfWork
    {
        ICurrentUser CurrentUser { get; }
        IIdentityRepository IdentityRepository { get; }
        IRoleRepository RoleRepository { get; }
        IAuditRepository AuditRepository { get; }
        IJwtTokenGenerator JwtTokenGenerator { get; }
        Task<IdentityUserPermissions?> IdentityUserAssingRoleAsync(Guid userId, Guid roleId, Guid oldRoleId);
    }
}
