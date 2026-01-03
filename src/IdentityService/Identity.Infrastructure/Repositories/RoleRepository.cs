using AbstractionBlocks.Common.Infrastructure;
using AbstractionBlocks.Common.Infrastructure.Persistance;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using MongoDB.Driver;

namespace IdentityService.Identity.Infrastructure.Repositories
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(MongoDatabase<Role> database) : base(database)
        {
        }

        public async Task<IEnumerable<string>?> RolesOfUserAsync(IEnumerable<Guid> userIds)
        {
            var result = await _collection.Collection.Find(role => userIds.Contains(role.Id)).
            Project(role => role.Name).
            ToListAsync();
            return result;
        }
    }
}
