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
}