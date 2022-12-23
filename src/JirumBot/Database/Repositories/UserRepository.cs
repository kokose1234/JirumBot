using JirumBot.Database.Entities;
using JirumBot.Database.Interfaces;
using MongoDB.Driver;

namespace JirumBot.Database.Repositories;

public class UserRepository : MongoRepository<UserEntity>
{
    private IMongoContext Context { get; }

    public UserRepository(IMongoContext context) : base(context.GetCollection<UserEntity>("users"))
    {
        Context = context;
    }

    public async Task<List<UserEntity>> All() => await Collection.Find(Builders<UserEntity>.Filter.Empty).ToListAsync();

    public async Task<UserEntity> GetByUserId(ulong id)
    {
        var builder = Builders<UserEntity>.Filter;
        var filter = builder.Eq(x => x.UserId, id);

        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<UserEntity> GetByChannelId(ulong id)
    {
        var builder = Builders<UserEntity>.Filter;
        var filter = builder.Eq(x => x.ChannelId, id);

        return await Collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task Create(UserEntity user) => await InsertOneAsync(user);

    public async Task Delete(UserEntity user) => await DeleteOneAsync(user);

    public async Task AddKeyword(ulong id, string keyword)
    {
        var user = await GetByUserId(id);
        user.Keywords.Add(keyword);

        await ReplaceOneAsync(user);
    }

    public async Task DeleteKeyword(ulong id, string keyword)
    {
        var user = await GetByUserId(id);
        user.Keywords.Remove(keyword);

        await ReplaceOneAsync(user);
    }

    public async Task ClearKeyword(ulong id)
    {
        var user = await GetByUserId(id);
        user.Keywords.Clear();

        await ReplaceOneAsync(user);
    }
}