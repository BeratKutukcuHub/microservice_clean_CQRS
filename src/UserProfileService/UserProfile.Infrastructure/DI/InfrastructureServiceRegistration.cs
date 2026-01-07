using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
using AbstractionBlocks.Common.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using UserProfileService.Application.Interfaces;
using UserProfileService.Infrastructure.Repositories;
namespace UserProfileService.Infrastructure.DI;
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddUserProfileInfrastructureDIServices(this IServiceCollection services)
    {
        services.AddDIEnjectionServices(
            "UserProfileDatabase",
            new Type[] { typeof(UserProfileService.Domain.Entities.UserProfile), typeof(AuditLog) }
        );
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        return services;
    }
}
