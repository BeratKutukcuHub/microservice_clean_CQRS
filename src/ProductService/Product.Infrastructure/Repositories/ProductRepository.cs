using AbstractionBlocks.Common.Infrastructure;
using AbstractionBlocks.Common.Infrastructure.Persistance;
using AbstractionBlocks.Common.Pagination;
using MongoDB.Driver;
using ProductService.Product.Application.Repository;
using ProductService.Product.Infrastructure.Extensions;
using ProductEntity = ProductService.Product.Domain.Product;
namespace ProductService.Product.Infrastructure.Repositories
{
    public class ProductRepository : Repository<ProductEntity>, IProductRepository
    {
        public ProductRepository(MongoDatabase<ProductEntity> database) : base(database)
        {
            EnsureIndexes();
        }
        private void EnsureIndexes()
        {
            var indexKeys = Builders<ProductEntity>.IndexKeys.Ascending(x => x.Name);
            var indexOptions = new CreateIndexOptions
            {
                Unique = true
            };
            var indexModel = new CreateIndexModel<ProductEntity>(indexKeys, indexOptions);
            _collection.Collection.Indexes.CreateOne(indexModel);
        }
        public async Task<PaginationResponse<ProductEntity>?> GetAllPagination(PaginationValue paginationValue)
        {
            return await _collection.Collection.GetAllPaginationAsync(paginationValue);
        }
        public async Task<IEnumerable<ProductEntity>> GetActiveProductsAsync()
        {
            return await _collection.Collection
                .Find(x => x.IsActive && !x.IsDeleted)
                .ToListAsync();
        }
    }
}
