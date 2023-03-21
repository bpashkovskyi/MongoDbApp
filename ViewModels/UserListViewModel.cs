namespace MongoDbApp.ViewModels;

using MongoDB.Bson;

public class UserListViewModel
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }
}