using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Exception.Logger;
using AbstractionBlocks.Common.Infrastructure.Caching;
using AbstractionBlocks.Common.Infrastructure.Concreate;
using Microsoft.Extensions.DependencyInjection;
namespace AbstractionBlocks.Common.Infrastructure.DI
{
    public static class DICommonInfrastructure
    {
        public static IServiceCollection AddDICommonInfrastructure(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCacheService>();
            services.AddSingleton(typeof(ILoggerService<>), typeof(LoggerService<>));
            services.AddScoped<ICurrentUser, CurrentUser>();
            return services;
        }
    }
}
