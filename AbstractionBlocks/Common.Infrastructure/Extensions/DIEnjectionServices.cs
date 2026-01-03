using AbstractionBlocks.Common.SecretBase.Options;
using AbstractionBlocks.Common.SecretBase.Provider;
using AbstractionBlocks.Common.Infrastructure.Persistance;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AbstractionBlocks.Common.Infrastructure.Extensions
{
    public static class DIEnjectionServices
    {
        public static IServiceCollection AddDIEnjectionServices(
    this IServiceCollection services,
    string databaseName,
    Type[] mongoDatabases)
    {
    services.AddSingleton<ISecretProvider<MongoDBOptions>, SecretProvider<MongoDBOptions>>();

    services.AddSingleton<IMongoClient>(sp =>
    {
        var opt = sp.GetRequiredService<ISecretProvider<MongoDBOptions>>().GetSection();
        MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
        var settings = MongoClientSettings.FromConnectionString(opt.ConnectionString);
        return new MongoClient(settings);
    });

    services.AddSingleton(sp =>
        sp.GetRequiredService<IMongoClient>().GetDatabase(databaseName));

    foreach (var mongoDatabase in mongoDatabases)
    {
        services.AddSingleton(typeof(MongoDatabase<>).MakeGenericType(mongoDatabase), sp =>
        {
            var db = sp.GetRequiredService<IMongoDatabase>();
            return Activator.CreateInstance(typeof(MongoDatabase<>).MakeGenericType(mongoDatabase), db);
        });
    }
    return services;

    }
    }
}