using MongoDB.Driver;

namespace JirumBot.Database.Interfaces;

public interface IMongoContext
{
    public IMongoCollection<T> GetCollection<T>(string collection);
}