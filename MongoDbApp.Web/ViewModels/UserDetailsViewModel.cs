using MongoDB.Bson;

namespace MongoDbApp.Web.ViewModels;

public class UserDetailsViewModel
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
}