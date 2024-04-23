using MongoDB.Bson;

namespace MongoDbApp.Web.Model;

public class User
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
}