using AbstractionBlocks.Common.Infrastructure.Extensions;
using IdentityService.Application.UOW;
using IdentityService.Identity.Infrastructure.Repositories;
using IdentityService.Identity.Infrastructure.UOW;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Infrastructure.Concreate;
using Microsoft.Extensions.DependencyInjection;
using AbstractionBlocks.Common.Domain;
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
            services.AddScoped<IdentityService.Application.UOW.IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
