using System.Linq.Expressions;
using AbstractionBlocks.Common.Application.Repository;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Infrastructure.Persistance;
using MongoDB.Driver;
namespace AbstractionBlocks.Common.Infrastructure
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    public readonly MongoDatabase<TEntity> _collection;
        public Repository(MongoDatabase<TEntity> collection)
        {
            _collection = collection;
        }
        public virtual async Task<Guid> AddAsync(TEntity entity)
    {
        await _collection.Collection.InsertOneAsync(entity);
        return entity.Id;
    }
    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _collection.Collection.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _collection.Collection.Find(_ => true).ToListAsync();
    }
    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _collection.Collection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        await _collection.Collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);
        return entity;
    }
    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _collection.Collection.Find(predicate).ToListAsync();
    }
}
}
