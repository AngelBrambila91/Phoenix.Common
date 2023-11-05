using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using MongoDB.Bson.IO;
using MongoDB.Driver;
namespace Phoenix.Common.MongoDB;

/*
MongoDB is a document-oriented NoSQL database which stores data in JSON-like documents with
dynamic schema
We will prefer a NoSQL solution (as opposed to a relational database) for our microservices because:
• We won’t need relationships across the data, because each microservice manages its own database
• We don’t need ACID guarantees, where ACID stands for atomicity, consistency, isolation and
durability, which are properties of database transactions that we won’t need in our services.
• We won’t need to write complex queries, since most of our service queries will be able to find
everything they need in a single document type
• Need low latency, high availability and high scalability, which are classic features of NoSQL
databases
*/
public class MongoRepository<T> : IRepository<T> where T : IEntity
{
    //Implemnt Dependecy Injection
    private readonly IMongoCollection<T> dbCollection;
    private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        dbCollection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
        return await dbCollection.Find<T>(filter).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        await dbCollection!.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        await dbCollection!.ReplaceOneAsync(filter, entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, id);
        await dbCollection!.DeleteOneAsync(filter);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await dbCollection.Find(filter).ToListAsync();
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}