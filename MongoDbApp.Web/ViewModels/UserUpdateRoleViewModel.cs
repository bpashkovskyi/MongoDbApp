using System.ComponentModel.DataAnnotations;

namespace MongoDbApp.ViewModels;

public class UserUpdateRoleViewModel
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Role { get; set; }
}