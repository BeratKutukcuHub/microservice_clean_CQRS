using MediatR;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;
namespace AbstractionBlocks.Common.Application.Caching
{
    public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;
        public CachingBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheAttribute = request.GetType().GetCustomAttribute<CacheAttribute>();
            if (cacheAttribute == null)
            {
                return await next();
            }
            var cacheKey = GenerateCacheKey(request, cacheAttribute.Key);
            var cachedResponse = await _cacheService.GetAsync<TResponse>(cacheKey);
            if (cachedResponse is not null)
            {
                _logger.LogInformation($"Fetching from cache for key: {cacheKey}");
                return cachedResponse;
            }
            var response = await next();
            await _cacheService.SetAsync(cacheKey, response, TimeSpan.FromMinutes(cacheAttribute.DurationInMinutes));
            _logger.LogInformation($"Added to cache for key: {cacheKey}");
            return response;
        }
        private string GenerateCacheKey(TRequest request, string keyPrefix)
        {
            var properties = request.GetType().GetProperties()
                .Select(p => $"{p.Name}:{p.GetValue(request)}")
                .ToArray();
            return $"{keyPrefix}:{string.Join("-", properties)}";
        }
    }
}
