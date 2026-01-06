using IdentityService.Application.Auth.Identity.Commands;
using IdentityService.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using IdentityService.Application.Helper;
using IdentityService.Application.Interfaces;
using IdentityService.Identity.Domain.Events;
using IdentityService.Application.Auth.Identity.Events;

namespace IdentityService.Application.DI
{
    public static class IdentityApplicationDIServices 
    {
        public static IServiceCollection AddIdentityApplicationDIServices(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(typeof(Profiles)));
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateIdentityCommand>());
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IApplicationDispatcher, ApplicationDispatcher>();
            services.AddScoped<IEventApplicationHandler<AuditLogEventDomain>,AuditLogEventHandler>();
            
            return services;
        } 
    }
}
