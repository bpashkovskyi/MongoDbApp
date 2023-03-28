namespace MongoDbApp.Repositories;

using MongoDB.Bson;

using Model;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserAsync(ObjectId id);
}