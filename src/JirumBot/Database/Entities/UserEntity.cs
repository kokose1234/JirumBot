using JirumBot.Database.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JirumBot.Database.Entities;

public sealed record UserEntity : IMongoEntity
{
    [BsonId]
    public ObjectId Id { get; init; }

    [BsonElement("userId")]
    public ulong UserId { get; init; }

    [BsonElement("channelId")]
    public ulong ChannelId { get; init; }

    [BsonElement("keywords")]
    public IList<string> Keywords { get; init; } = new List<string>();
}