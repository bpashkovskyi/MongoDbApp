using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;

using MongoDB.Bson;
using MongoDB.Driver;

using MongoDbApp.Web.Model;

namespace MongoDbApp.Web.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> userCollection;
    private readonly IDistributedCache distributedCache;

    public UserRepository(
        IMongoDatabase mongoDatabase,
        IDistributedCache distributedCache)
    {
        userCollection = mongoDatabase.GetCollection<User>("Users");
        this.distributedCache = distributedCache;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await userCollection.AsQueryable().ToListAsync();
    }

    public async Task<User> GetUserAsync(ObjectId id)
    {
        var user = await GetUserFromCacheAsync(id);
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
        await userCollection.InsertOneAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        var filter = Builders<User>.Filter
                .Eq(u => u.Id, user.Id);

        var update = Builders<User>.Update
            .Set(u => u.Name, user.Name)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Role, user.Role);

        await userCollection.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(user.Id);
    }

    public async Task UpdateUserNameAsync(ObjectId id, string name)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        var update = Builders<User>.Update
            .Set(u => u.Name, name);

        await userCollection.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(id);
    }

    public async Task UpdateUserRoleAsync(ObjectId id, string role)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        var update = Builders<User>.Update
            .Set(u => u.Role, role);

        await userCollection.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(id);
    }

    public async Task DeleteUserAsync(ObjectId id)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        await userCollection.DeleteOneAsync(filter);

        await ClearUserFromCacheAsync(id);
    }

    private async Task<User> GetUserFromDatabaseAsync(ObjectId id)
    {
        return await userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    private async Task<User> GetUserFromCacheAsync(ObjectId id)
    {
        var cacheKey = id.ToString();

        var userAsString = await distributedCache.GetStringAsync(cacheKey);
        if (userAsString is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<User>(userAsString);
    }

    private async Task SetUserToCacheAsync(User user)
    {
        var cacheKey = user.Id.ToString();

        var userAsString = JsonSerializer.Serialize(user);

        var distributedCacheEntryOptions = new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(1) };
        await distributedCache.SetStringAsync(cacheKey, userAsString, distributedCacheEntryOptions);
    }

    private async Task ClearUserFromCacheAsync(ObjectId id)
    {
        var cacheKey = id.ToString();

        await distributedCache.RemoveAsync(cacheKey);
    }
}