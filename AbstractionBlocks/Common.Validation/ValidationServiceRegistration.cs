using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace AbstractionBlocks.Common.Validation
{
    public static class ValidationServiceRegistration
    {
        public static IServiceCollection AddValidationInfrastructure(this IServiceCollection services, Assembly assembly)
        {
            services.AddValidatorsFromAssembly(assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }
    }
}
