using AbstractionBlocks.Common.Infrastructure.Extensions;
using AbstractionBlocks.Common.Domain;
using Microsoft.Extensions.DependencyInjection;
using AbstractionBlocks.Common.Application.Interfaces;
using ProductService.Product.Application.Repository;
using ProductService.Product.Infrastructure.Repositories;
using ProductService.Product.Infrastructure.UOW;
using ProductService.Product.Infrastructure.Concreate;
namespace ProductService.Product.Infrastructure.DI
{
    public static class ProductInfrastructureDIServices
    {
        public static IServiceCollection AddProductInfrastructureDIServices(this IServiceCollection services)
        {
            services.AddDIEnjectionServices("ProductDatabase", new Type[]
            {
                typeof(Domain.Product),
                typeof(AuditLog)
            });
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<Application.UOW.IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
