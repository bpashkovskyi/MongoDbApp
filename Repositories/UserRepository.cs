namespace MongoDbApp.Repositories;

using Microsoft.Extensions.Options;

using MongoDB.Bson;
using MongoDB.Driver;

using Model;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IOptions<MongoDbSettings> settingsOptions)
    {
        var settings = settingsOptions.Value;

        var client = new MongoClient(settings.ConnectionString);
        var userDatabase = client.GetDatabase(settings.UserDatabaseName);

        this._users = userDatabase.GetCollection<User>("Users");
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await this._users.AsQueryable().ToListAsync();
    }

    public async Task<User> GetUserAsync(ObjectId id)
    {
        return await this._users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    public async Task AddUserAsync(User user)
    {
        await this._users.InsertOneAsync(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        var filter = Builders<User>.Filter
                .Eq(u => u.Id, user.Id);

        var update = Builders<User>.Update
            .Set(u => u.Name, user.Name)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Role, user.Role);

        await this._users.UpdateOneAsync(filter, update);
    }

    public async Task UpdateUserNameAsync(ObjectId id, string name)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        var update = Builders<User>.Update
            .Set(u => u.Name, name);

        await this._users.UpdateOneAsync(filter, update);
    }

    public async Task UpdateUserRoleAsync(ObjectId id, string role)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        var update = Builders<User>.Update
            .Set(u => u.Role, role);

        await this._users.UpdateOneAsync(filter, update);
    }

    public async Task DeleteUserAsync(ObjectId id)
    {
        var filter = Builders<User>.Filter
            .Eq(u => u.Id, id);

        await this._users.DeleteOneAsync(filter);
    }
}