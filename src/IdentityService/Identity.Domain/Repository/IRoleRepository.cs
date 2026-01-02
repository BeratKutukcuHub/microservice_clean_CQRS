using AbstractionBlocks.CommonDomain.Repository;

namespace IdentityService.Identity.Domain.Repository
{
    public interface IRoleRepository : IRepository<Role>
    {
        public Task<IEnumerable<string>?> RolesOfUserAsync(IEnumerable<Guid> userIds);
    }
}
