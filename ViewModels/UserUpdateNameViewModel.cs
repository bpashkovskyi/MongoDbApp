using System.ComponentModel.DataAnnotations;

namespace MongoDbApp.ViewModels;

public class UserUpdateNameViewModel
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }
}