using MongoDB.Bson;

namespace JirumBot.Database.Interfaces;

public interface IMongoEntity
{
    ObjectId Id { get; init; }
}