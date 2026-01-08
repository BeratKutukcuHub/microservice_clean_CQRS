using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Application.Caching;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Infrastructure.Concreate;
using AbstractionBlocks.Common.Infrastructure.Extensions;
using MailNotification.Application.Interfaces;
using MailNotification.Infrastructure.Repositories;
using MailNotification.Infrastructure.Services;
using MailNotification.Infrastructure.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace MailNotification.Infrastructure.DI
{
    public static class MailNotificationInfrastructureDIServices
    {
        public static IServiceCollection AddMailNotificationInfrastructureDIServices(this IServiceCollection services)
        {
            services.AddDIEnjectionServices(
                "MailNotificationDatabase",
                new Type[] { typeof(AuditLog) }
            );

            services.AddScoped<IMailService, MailService>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IAuditRepository, AuditRepository>();

            // Add caching
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();

            return services;
        }
    }
}
