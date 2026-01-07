using AbstractionBlocks.Common.Pagination;
using Category.Application.Interfaces;
using MongoDB.Driver;
namespace Category.Infrastructure.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<Domain.Category> _collection;
    public CategoryRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Domain.Category>("Categories");
    }
    public async Task<Domain.Category?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(c => c.Id == id && !c.IsDeleted).FirstOrDefaultAsync();
    }
    public async Task<Domain.Category?> GetByNameAsync(string name)
    {
        return await _collection.Find(c => c.Name == name && !c.IsDeleted).FirstOrDefaultAsync();
    }
    public async Task<List<Domain.Category>> GetAllAsync()
    {
        return await _collection.Find(c => !c.IsDeleted).ToListAsync();
    }
    public async Task<PaginationResponse<Domain.Category>> GetAllPaginationAsync(PaginationValue pagination)
    {
        var filter = Builders<Domain.Category>.Filter.Eq(c => c.IsDeleted, false);
        var totalCount = await _collection.CountDocumentsAsync(filter);
        var categories = await _collection
            .Find(filter)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pagination.PageSize);
        return PaginationResponse<Domain.Category>.Create(
            pagination.PageNumber,
            pagination.PageSize,
            (int)totalCount,
            totalPages,
            categories
        );
    }
    public async Task<Guid> AddAsync(Domain.Category category)
    {
        await _collection.InsertOneAsync(category);
        return category.Id;
    }
    public async Task<bool> UpdateAsync(Domain.Category category)
    {
        var result = await _collection.ReplaceOneAsync(
            c => c.Id == category.Id,
            category
        );
        return result.ModifiedCount > 0;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category == null) return false;
        category.SoftDelete();
        return await UpdateAsync(category);
    }
}
