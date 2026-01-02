using IdentityService.Application.Auth.Identity.Commands;
using IdentityService.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityService.Application.DI
{
    public static class IdentityApplicationDIServices
    {
        public static IServiceCollection AddIdentityApplicationDIServices(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(typeof(Profiles)));
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateIdentityCommand>());
            return services;
        } 
    }
}
