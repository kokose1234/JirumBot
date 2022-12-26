using System.Linq.Expressions;
using JirumBot.Database.Interfaces;
using MongoDB.Driver;

namespace JirumBot.Database;

public class MongoRepository<T> : IMongoRepository<T> where T : IMongoEntity
{
    public MongoRepository(IMongoCollection<T> collection) => Collection = collection;

    protected IMongoCollection<T> Collection { get; }


    public async Task DeleteOneAsync(T entity)
    {
        await Collection.DeleteOneAsync(x => x.Id == entity.Id);
    }

    public async Task DeleteOneAsync(Expression<Func<T, bool>> expression)
    {
        await Collection.DeleteOneAsync(expression);
    }

    public async Task<T> FindFirstOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        return await Collection.Find(expression).FirstOrDefaultAsync();
    }

    public T FirstOrDefault(Expression<Func<T, bool>> expression)
    {
        return Collection.FindSync(expression).FirstOrDefault();
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return await Collection.Find(expression).AnyAsync();
    }

    public async Task InsertOneAsync(T entity)
    {
        await Collection.InsertOneAsync(entity);
    }

    public async Task ReplaceOneAsync(T entity)
    {
        await Collection.ReplaceOneAsync(doc => doc.Id == entity.Id, entity);
    }

    public async Task DeleteManyAsync(Expression<Func<T, bool>> expression)
    {
        await Collection.DeleteManyAsync(expression);
    }

    public async Task<long> CountDocuments(Expression<Func<T, bool>> expression)
    {
        return await Collection.CountDocumentsAsync(expression);
    }

    public async Task<IAsyncCursor<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await Collection.FindAsync(expression);
    }
}