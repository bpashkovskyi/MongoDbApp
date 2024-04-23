namespace MongoDbApp.Repositories;

using MongoDB.Bson;
using MongoDB.Driver;

using Model;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IDistributedCache _distributedCache;

    public UserRepository(
        IMongoDatabase mongoDatabase,
        IDistributedCache distributedCache)
    {
        _userCollection = mongoDatabase.GetCollection<User>("Users");
        _distributedCache = distributedCache;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _userCollection.AsQueryable().ToListAsync();
    }

    public async Task<User> GetUserAsync(ObjectId id)
    {
        User user = await GetUserFromCacheAsync(id);
        if (user is not null)
        {
            return user;
        }

        user = await GetUserFromDatabaseAsync(id);

        if (user is not null)
        {
            await SetUserToCacheAsync(user);
        }

        return user;
    }

    public async Task AddUserAsync(User user)
    {
        await _userCollection.InsertOneAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        FilterDefinition<User> filter = Builders<User>.Filter
                .Eq(u => u.Id, user.Id);

        UpdateDefinition<User> update = Builders<User>.Update
            .Set(u => u.Name, user.Name)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Role, user.Role);

        await _userCollection.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(user.Id);
    }

    public async Task UpdateUserNameAsync(ObjectId id, string name)
    {
        FilterDefinition<User> filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        UpdateDefinition<User> update = Builders<User>.Update
            .Set(u => u.Name, name);

        await _userCollection.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(id);
    }

    public async Task UpdateUserRoleAsync(ObjectId id, string role)
    {
        FilterDefinition<User> filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        UpdateDefinition<User> update = Builders<User>.Update
            .Set(u => u.Role, role);

        await _userCollection.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(id);
    }

    public async Task DeleteUserAsync(ObjectId id)
    {
        FilterDefinition<User> filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        await _userCollection.DeleteOneAsync(filter);

        await ClearUserFromCacheAsync(id);
    }

    private async Task<User> GetUserFromDatabaseAsync(ObjectId id)
    {
        return await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    private async Task<User> GetUserFromCacheAsync(ObjectId id)
    {
        string cacheKey = id.ToString();

        string userAsString = await _distributedCache.GetStringAsync(cacheKey);
        if (userAsString is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<User>(userAsString);
    }

    private async Task SetUserToCacheAsync(User user)
    {
        string cacheKey = user.Id.ToString();

        string userAsString = JsonSerializer.Serialize(user);

        DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(1) };
        await _distributedCache.SetStringAsync(cacheKey, userAsString, distributedCacheEntryOptions);
    }

    private async Task ClearUserFromCacheAsync(ObjectId id)
    {
        string cacheKey = id.ToString();

        await _distributedCache.RemoveAsync(cacheKey);
    }
}