using System.Linq.Expressions;
namespace AbstractionBlocks.Common.Application.Repository
{
    public interface IRepository<TEntity>
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(Guid entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
