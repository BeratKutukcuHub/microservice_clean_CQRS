using AbstractionBlocks.Common.Application.Interfaces;
using IdentityService.Application.Helper;
using IdentityService.Identity.Application.Provider;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
namespace IdentityService.Application.UOW
{
    public interface IUnitOfWork : AbstractionBlocks.Common.Application.Interfaces.IUnitOfWork
    {
        ICurrentUser CurrentUser { get; }
        IIdentityRepository IdentityRepository { get; }
        IRoleRepository RoleRepository { get; }
        IAuditRepository AuditRepository { get; }
        IJwtTokenGenerator JwtTokenGenerator { get; }
        Task<IdentityUser> CreateIdentityUserAsync(IdentityUser user);
        Task<IdentityUser> UpdateIdentityUserAsync(IdentityUser user);
        Task<IdentityUser> DeleteIdentityUserAsync(Guid id);
        Task<IdentityUser> GetIdentityUserByIdAsync(Guid id);
        Task<List<IdentityUser>> GetAllIdentityUsersAsync();
        Task<(IdentityUser User, List<string> Permissions)> IdentityUserAssingRoleAsync(Guid userId, Guid roleId, Guid targetRoleId);
        Task<IdentityUser> BlockUserAsync(Guid userId);
    }
}
