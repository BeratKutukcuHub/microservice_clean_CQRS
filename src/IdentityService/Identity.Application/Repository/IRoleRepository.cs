using AbstractionBlocks.CommonDomain.Repository;
using IdentityService.Identity.Domain;

namespace IdentityService.Identity.Application.Repository
{
    public interface IRoleRepository : IRepository<Role>
    {
        Task<IEnumerable<string>?> RolesOfUserAsync(IEnumerable<Guid> userIds);
    }
}
