using MongoDB.Bson;

namespace MongoDbApp.Web.ViewModels;

public class UserListViewModel
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }
}