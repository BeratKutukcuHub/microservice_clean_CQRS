using AbstractionBlocks.Common.Application.Dispatchers;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using ProductService.Product.Application.Commands;
using ProductService.Product.Application.Events;
namespace ProductService.Product.Application.DI
{
    public static class ProductApplicationDIServices
    {
        public static IServiceCollection AddProductApplicationDIServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
                cfg.AddOpenBehavior(typeof(AbstractionBlocks.Common.Application.Caching.CachingBehavior<,>));
            });
            services.AddValidationInfrastructure(typeof(CreateProductCommand).Assembly);
            services.AddScoped<IApplicationDispatcher, ApplicationDispatcher>();
            services.AddScoped<IEventApplicationHandler<AuditLogEventDomain>, AuditLogEventHandler>();
            return services;
        }
    }
}
