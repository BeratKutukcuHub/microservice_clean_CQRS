using AbstractionBlocks.Common.Pagination;
using AbstractionBlocks.Common.Infrastructure;
using AbstractionBlocks.Common.Infrastructure.Persistance;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using IdentityService.Identity.Infrastructure.Extensions;
using MongoDB.Driver;
using IdentityService.Application.Exceptions;

namespace IdentityService.Identity.Infrastructure.Repositories
{
    public class IdentityUserRepository : Repository<IdentityUser>, IIdentityRepository
    {
        public IdentityUserRepository(MongoDatabase<IdentityUser> database) : base(database)
        {
            EnsureIndexes();
        }
        private void EnsureIndexes()
        {
            var indexKeys = Builders<IdentityUser>.IndexKeys.Ascending(x => x.Email);

            var indexOptions = new CreateIndexOptions
            {
                Unique = true
            };
            var indexModel = new CreateIndexModel<IdentityUser>(indexKeys, indexOptions);
            _collection.Collection.Indexes.CreateOne(indexModel);
        }
        public async Task<PaginationResponse<IdentityUser>?> GetAllPagination(PaginationValue paginationValue) =>
        await _collection.Collection.GetAllPaginationAsync(paginationValue);
        public async Task<IdentityUser> GetByIdSessionAsync(IClientSessionHandle session,Guid id, Guid roleId, Guid oldRoleId)
        {
            var user = await _collection.Collection.Find(session, x => x.Id == id).FirstOrDefaultAsync();
            if (user == null) throw new NotFoundExceptionApp($"IdentityUser with ID {id} not found.");
            user.RemoveRole(oldRoleId);
            user.AddRole(roleId);
            user.AddRefreshToken();
            await _collection.Collection.ReplaceOneAsync(x => x.Id == id, user);
            return user;
        }
    }
}
