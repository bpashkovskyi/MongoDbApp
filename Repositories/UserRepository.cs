namespace MongoDbApp.Repositories;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using Model;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;
    private readonly IDistributedCache _distributedCache;

    public UserRepository(
        IOptions<MongoDbSettings> settingsOptions,
        IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
        var settings = settingsOptions.Value;

        var client = new MongoClient(settings.ConnectionString);
        var userDatabase = client.GetDatabase(settings.UserDatabaseName);

        _users = userDatabase.GetCollection<User>("Users");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _users.AsQueryable().ToListAsync();
    }

    public async Task<User> GetUserAsync(ObjectId id)
    {
        var user = await GetUserFromCacheAsync(id);
        if (user is not null)
        {
            return user;
        }

        user = await GetUserFromDatabaseAsync(id);
        await SetUserToCacheAsync(user);

        return user;
    }

    public async Task AddUserAsync(User user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        var filter = Builders<User>.Filter
                .Eq(u => u.Id, user.Id);

        var update = Builders<User>.Update
            .Set(u => u.Name, user.Name)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Role, user.Role);

        await _users.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(user.Id);
    }

    public async Task UpdateUserNameAsync(ObjectId id, string name)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        var update = Builders<User>.Update
            .Set(u => u.Name, name);

        await _users.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(id);
    }

    public async Task UpdateUserRoleAsync(ObjectId id, string role)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        var update = Builders<User>.Update
            .Set(u => u.Role, role);

        await _users.UpdateOneAsync(filter, update);

        await ClearUserFromCacheAsync(id);
    }

    public async Task DeleteUserAsync(ObjectId id)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        await _users.DeleteOneAsync(filter);

        await ClearUserFromCacheAsync(id);
    }

    private async Task<User> GetUserFromDatabaseAsync(ObjectId id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    private async Task<User> GetUserFromCacheAsync(ObjectId id)
    {
        var cacheKey = id.ToString();

        var userAsString = await _distributedCache.GetStringAsync(cacheKey);
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
        await _distributedCache.SetStringAsync(cacheKey, userAsString, distributedCacheEntryOptions);
    }

    private async Task ClearUserFromCacheAsync(ObjectId id)
    {
        var cacheKey = id.ToString();

        await _distributedCache.RemoveAsync(cacheKey);
    }
}