using AbstractionBlocks.Common.Pagination;
using MongoDB.Driver;
namespace ProductService.Product.Infrastructure.Extensions
{
    public static class GetAllPaginationService
    {
        public static async Task<PaginationResponse<T>?> GetAllPaginationAsync<T>
        (this IMongoCollection<T> collection, PaginationValue pagination) where T : class
        {
            var totalCount = await collection.CountDocumentsAsync(FilterDefinition<T>.Empty);
            var skip = (pagination.PageNumber - 1) * pagination.PageSize;
            var items = await collection
                .Find(FilterDefinition<T>.Empty)
                .Skip(skip)
                .Limit(pagination.PageSize)
                .ToListAsync();
            return PaginationResponse<T>.Create(
                pagination.PageNumber,
                pagination.PageSize,
                (int)totalCount,
                (int)Math.Ceiling((double)totalCount / pagination.PageSize),
                items
            );
        }
    }
}
