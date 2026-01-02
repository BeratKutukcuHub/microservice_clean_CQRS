using AbstractionBlocks.CommonInfrastructure;
using AbstractionBlocks.CommonInfrastructure.Persistance;
using IdentityService.Identity.Domain;
using IdentityService.Identity.Domain.Repository;
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
        
    }
}
