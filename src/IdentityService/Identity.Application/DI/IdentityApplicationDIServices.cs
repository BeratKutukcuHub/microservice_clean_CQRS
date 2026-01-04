using IdentityService.Application.Auth.Identity.Commands;
using IdentityService.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Shared.Authentication;
using IdentityService.Application.Auth;
using IdentityService.Application.Interfaces;
using IdentityService.Application.Helper;

namespace IdentityService.Application.DI
{
    public static class IdentityApplicationDIServices
    {
        public static IServiceCollection AddIdentityApplicationDIServices(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(typeof(Profiles)));
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateIdentityCommand>());
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            return services;
        } 
    }
}
