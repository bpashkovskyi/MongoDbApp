namespace MongoDbApp.Repositories;

using MongoDB.Bson;

using Model;

public interface IUserRepository
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> GetUserAsync(ObjectId id);

    Task AddUserAsync(User user);

    Task UpdateUserAsync(User user);
    Task UpdateUserNameAsync(ObjectId id, string name);
    Task UpdateUserRoleAsync(ObjectId id, string role);
    Task DeleteUserAsync(ObjectId id);
}