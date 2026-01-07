using AbstractionBlocks.Common.Application.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Text.Json;
using System.Threading.Tasks;
namespace AbstractionBlocks.Common.Infrastructure.Caching
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        public Task<T?> GetAsync<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T? value);
            return Task.FromResult(value);
        }
        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask;
        }
        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                cacheOptions.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {
                cacheOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
            }
            _memoryCache.Set(key, value, cacheOptions);
            return Task.CompletedTask;
        }
    }
}
