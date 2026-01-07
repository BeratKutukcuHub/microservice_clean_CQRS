using AbstractionBlocks.Common.Pagination;
namespace Category.Application.Interfaces;
public interface ICategoryRepository
{
    Task<Domain.Category?> GetByIdAsync(Guid id);
    Task<Domain.Category?> GetByNameAsync(string name);
    Task<List<Domain.Category>> GetAllAsync();
    Task<PaginationResponse<Domain.Category>> GetAllPaginationAsync(PaginationValue pagination);
    Task<Guid> AddAsync(Domain.Category category);
    Task<bool> UpdateAsync(Domain.Category category);
    Task<bool> DeleteAsync(Guid id);
}
