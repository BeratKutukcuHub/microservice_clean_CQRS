using AbstractionBlocks.Common.Infrastructure.Extensions;
using IdentityService.Application.Interfaces;
using IdentityService.Application.UOW;
using IdentityService.Identity.Infrastructure.Repositories;
using IdentityService.Identity.Infrastructure.UOW;
using IdentityService.Infrastructure.Concreate;
using Microsoft.Extensions.DependencyInjection;
using IdentityService.Identity.Domain;
using IdentityService.Identity.Application.Repository;

namespace IdentityService.Identity.Infrastructure.DI
{
    public static class IdentityInfrastructureDIServices
    {
        public static IServiceCollection AddIdentityInfrastructureDIServices(this IServiceCollection services)
        {
            services.AddDIEnjectionServices
            ("IdentityDatabase", new Type[] { typeof(IdentityUser), typeof(Role), typeof(AuditLog) });
            services.AddScoped<IIdentityRepository, IdentityUserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
