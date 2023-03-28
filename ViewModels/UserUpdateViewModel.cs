using MongoDB.Bson;

namespace MongoDbApp.ViewModels;

public class UserUpdateViewModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
}