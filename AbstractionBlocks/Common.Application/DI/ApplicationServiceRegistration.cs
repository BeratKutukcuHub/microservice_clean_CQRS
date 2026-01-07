using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Application.Dispatchers;
using AbstractionBlocks.Common.Application.Events;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
namespace AbstractionBlocks.Common.Application.DI
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddCommonApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IApplicationDispatcher, ApplicationDispatcher>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddScoped<IEventApplicationHandler<AuditLogEventDomain>, AuditLogEventHandler>();
            return services;
        }
    }
}
