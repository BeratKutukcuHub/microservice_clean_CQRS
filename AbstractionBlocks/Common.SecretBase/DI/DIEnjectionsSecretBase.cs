using AbstractionBlocks.Common.SecretBase.Options;
using AbstractionBlocks.Common.SecretBase.Provider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AbstractionBlocks.Common.SecretBase.DI
{
    public static class DIEnjectionsSecretBase
    {
        public static IServiceCollection AddDIEnjectionsSecretBase(this IServiceCollection services)
        {
            new ConfigurationBuilder().AddJsonFile("secretbase.json", optional: true, reloadOnChange: true)
            .Build();

            services.AddSingleton(typeof(ISecretProvider<>), typeof(SecretProvider<>));
            return services;
        }
    }
}