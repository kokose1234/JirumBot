using System.Linq.Expressions;

namespace JirumBot.Database.Interfaces;

public interface IMongoRepository<T>
{
    T FirstOrDefault(Expression<Func<T, bool>> expression);

    Task<T> FindFirstOrDefaultAsync(Expression<Func<T, bool>> expression);

    Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

    Task InsertOneAsync(T entity);

    Task DeleteOneAsync(T entity);

    Task DeleteOneAsync(Expression<Func<T, bool>> expression);

    Task ReplaceOneAsync(T entity);
}