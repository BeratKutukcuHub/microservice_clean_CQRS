using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Infrastructure.Caching;
using AbstractionBlocks.Common.Infrastructure.Extensions;
using Category.Application.Interfaces;
using Category.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
namespace Category.Infrastructure.DI;
public static class CategoryInfrastructureDIServices
{
    public static IServiceCollection AddCategoryInfrastructureServices(this IServiceCollection services)
    {
        services.AddDIEnjectionServices(
            "CategoryDatabase",
            new Type[] { typeof(Domain.Category), typeof(AuditLog) }
        );
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddSingleton<ICacheService, InMemoryCacheService>();
        return services;
    }
}
