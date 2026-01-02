using AbstractionBlocks.CommonApplication.Pagination;
using MongoDB.Driver;

namespace IdentityService.Identity.Infrastructure.Extensions
{
    public static class GetAllPaginationService
    {
        public static async Task<PaginationResponse<T>?> GetAllPaginationAsync<T>
        (this IMongoCollection<T> repository, PaginationValue pagination) where T : class, new()
        {
            var totalCount = await repository.CountDocumentsAsync(FilterDefinition<T>.Empty);

            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
        
            var items = await repository
                .Find(FilterDefinition<T>.Empty)
                .Skip(skip)
                .Limit(pagination.PageSize)
                .ToListAsync();
        
            return PaginationResponse<T>.Create(
                pagination.PageNumber,
                pagination.PageSize,
                items.Count,
                (int)totalCount,
                items
            );
        }
    }
}