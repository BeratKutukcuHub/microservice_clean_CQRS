using AbstractionBlocks.CommonApplication.Pagination;
using AbstractionBlocks.CommonInfrastructure;
using AbstractionBlocks.CommonInfrastructure.Persistance;
using IdentityService.Identity.Application.Repository;
using IdentityService.Identity.Domain;
using IdentityService.Identity.Infrastructure.Extensions;
using MongoDB.Driver;

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
    }
}
