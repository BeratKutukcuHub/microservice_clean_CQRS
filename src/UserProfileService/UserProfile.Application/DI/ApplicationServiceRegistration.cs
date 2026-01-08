using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AbstractionBlocks.Common.Validation;
using AbstractionBlocks.Common.Application.DI;

namespace UserProfileService.Application.DI;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddUserProfileApplicationDIServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidationInfrastructure(Assembly.GetExecutingAssembly());
        services.AddCommonApplicationServices();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(AbstractionBlocks.Common.Application.Caching.CachingBehavior<,>));
        });

        return services;
    }
}
