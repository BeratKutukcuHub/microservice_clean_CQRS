using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AbstractionBlocks.Common.Messaging.Configuration;
using AbstractionBlocks.Common.Messaging.Interfaces;
using AbstractionBlocks.Common.Messaging.RabbitMQ;

namespace AbstractionBlocks.Common.Messaging.DI;

public static class MessagingServiceExtensions
{
    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "RabbitMQ")
    {
        services.Configure<RabbitMQSettings>(configuration.GetSection(sectionName));
        
        services.AddSingleton<RabbitMQConnection>();
        services.AddSingleton<IEventBus, RabbitMQEventBus>();
        
        return services;
    }

    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        Action<RabbitMQSettings> configureOptions)
    {
        services.Configure(configureOptions);
        
        services.AddSingleton<RabbitMQConnection>();
        services.AddSingleton<IEventBus, RabbitMQEventBus>();
        
        return services;
    }

    public static IServiceCollection AddEventHandler<THandler>(this IServiceCollection services)
        where THandler : class
    {
        services.AddScoped(typeof(THandler));
        return services;
    }
}
