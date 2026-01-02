using System;
using AbstractBlocks.CommonDomain.Logger;
using AbstractionBlocks.CommonInfrastructure.Logger;
using AbstractionBlocks.CommonInfrastructure.Persistance;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace AbstractionBlocks.DIEnjections
{
    public static class DIEnjectionServices
    {
        public static IServiceCollection AddDIEnjectionsServices(this IServiceCollection services, string dbName, params Type[] repoTypes)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

            services.AddSingleton<IMongoClient>(x => new MongoClient("mongodb://localhost:27017"));
            services.AddScoped(x => x.GetRequiredService<IMongoClient>().GetDatabase(dbName));

            if (repoTypes == null || repoTypes.Length == 0)
                throw new ArgumentException("At least one entity type must be provided to register MongoDatabase<T> and ILoggerService<T>.");

            foreach (Type entityType in repoTypes)
            {
                if (entityType.IsInterface || entityType.IsAbstract)
                    throw new ArgumentException($"Type '{entityType.FullName}' must be a concrete entity class. Pass the entity types (e.g. 'IdentityUser'), not repository interfaces.");

                var mongoDatabaseType = typeof(MongoDatabase<>).MakeGenericType(entityType);

                services.AddScoped(mongoDatabaseType, sp =>
                {
                    var database = sp.GetRequiredService<IMongoClient>().GetDatabase(dbName);
                    return Activator.CreateInstance(mongoDatabaseType, database)!;
                });

            }

            services.AddScoped(typeof(ILoggerService<>), typeof(LoggerService<>));

            try
            {
                using (var sp = services.BuildServiceProvider())
                {
                    foreach (var entityType in repoTypes)
                    {
                                var mongoType = typeof(MongoDatabase<>).MakeGenericType(entityType);
                        sp.GetRequiredService(mongoType);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("DI validation failed in AddDIEnjectionsServices. Ensure you passed concrete entity types and that constructors match expected signatures. See inner exception for details.", ex);
            }

            return services;
        }
    }
}
