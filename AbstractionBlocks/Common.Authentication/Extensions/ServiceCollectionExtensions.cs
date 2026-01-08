using System.Text;
using AbstractionBlocks.Common.SecretBase.Options;
using AbstractionBlocks.Common.SecretBase.Provider;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
namespace Shared.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDICommonAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication()
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                ISecretProvider<JwtOptions> provider = new SecretProvider<JwtOptions>(configuration);
                var secretProvider = provider.GetSection();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = secretProvider.Issuer,
                    ValidAudience = secretProvider.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        secretProvider.SecretKey))
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("NotBlocked", policy =>
                    policy.RequireAssertion(context =>
                    {
                        var isBlocked = context.User.FindFirst("IsBlocked")?.Value;
                        return isBlocked != "true";
                    }));
            });
            return services;
        }
    }
}
