using MongoDB.Driver;

namespace AbstractionBlocks.Common.Infrastructure.Persistance
{

    public class MongoDatabase<TEntity>
    {
        private readonly IMongoDatabase _database;
        public IMongoCollection<TEntity> Collection => _database.GetCollection<TEntity>(GetCollectionName());

        public MongoDatabase(IMongoDatabase database)
        {
            _database = database;
        }

        private string GetCollectionName()
        {
            var name = typeof(TEntity).Name;
            return name.EndsWith("s") ? name : name + "s";
        }
    }
}
