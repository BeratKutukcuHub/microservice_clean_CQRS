using AbstractionBlocks.Common.Application.DI;
using AbstractionBlocks.Common.Validation;
using MailNotification.Application.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace MailNotification.Application.DI
{
    public static class MailNotificationApplicationDIServices
    {
        public static IServiceCollection AddMailNotificationApplicationDIServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(SendMailCommand).Assembly));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<SendMailCommand>();
                cfg.AddOpenBehavior(typeof(AbstractionBlocks.Common.Application.Caching.CachingBehavior<,>));
            });

            services.AddValidationInfrastructure(typeof(SendMailCommand).Assembly);
            services.AddCommonApplicationServices();

            return services;
        }
    }
}
