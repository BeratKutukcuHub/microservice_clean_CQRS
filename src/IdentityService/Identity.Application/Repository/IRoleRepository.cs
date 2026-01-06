using AbstractionBlocks.Common.Application.Repository;
using IdentityService.Identity.Domain;

namespace IdentityService.Identity.Application.Repository
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<IEnumerable<string>?> RolesOfUserAsync(IEnumerable<Guid> userIds);
        Task<List<string>> GetAllPermissionsAsync(List<Guid> roleIds);
    }
}
