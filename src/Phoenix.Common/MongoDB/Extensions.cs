using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Phoenix.Common.Settings;

namespace Phoenix.Common.MongoDB;

public static partial class WebApplicationBuilder
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        /*
To fix the json representation of our gods we just need to register a couple of MongoDB
serializers. We can do this in the Program.cs, so for now let’s just
think of Program.cs as the place where we register services that can be used across the
entire application.
*/
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetService<IConfiguration>();
            var mongoDbSettings = configuration!.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            var mongoClient = new MongoClient(mongoDbSettings!.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings!.ServiceName);
        });

        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName) where T : IEntity
    {
        services.AddSingleton<IRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetService<IMongoDatabase>();
            return new MongoRepository<T>(database!, collectionName);
        });

        return services;
    }
}