using AbstractionBlocks.Common.Infrastructure;
using AbstractionBlocks.Common.Infrastructure.Persistance;
using IdentityService.Application.Exceptions;
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
        public async Task<Role> GetByIdSessionAsync(IClientSessionHandle session, Guid id)
        {
            var role = await _collection.Collection.Find(session, x => x.Id == id).FirstOrDefaultAsync();
            if (role == null) throw new NotFoundExceptionApp($"Role with ID {id} not found.");
            return role;
        }
        public async Task<List<string>> GetAllPermissionsAsync(List<Guid> roleIds)
        {
            var result = await _collection.Collection.Find(role => roleIds.Contains(role.Id)).
            Project(role => role.Permissions).
            ToListAsync();
            return result.SelectMany(x => x).ToList();
        }
    }
}
