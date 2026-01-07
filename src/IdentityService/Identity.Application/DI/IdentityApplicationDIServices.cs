using IdentityService.Application.Auth.Identity.Commands;
using IdentityService.Application.Mappings;
using AbstractionBlocks.Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using IdentityService.Application.Helper;
using AbstractionBlocks.Common.Application.DI;
using AbstractionBlocks.Common.Application.Interfaces;
using AbstractionBlocks.Common.Domain;
namespace IdentityService.Application.DI
{
    public static class IdentityApplicationDIServices
    {
        public static IServiceCollection AddIdentityApplicationDIServices(this IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfile(typeof(Profiles)));
            services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<CreateIdentityCommand>());
            services.AddValidationInfrastructure(typeof(CreateIdentityCommand).Assembly);
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddCommonApplicationServices();
            return services;
        }
    }
}
