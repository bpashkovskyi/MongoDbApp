using System.ComponentModel.DataAnnotations;

namespace MongoDbApp.Web.ViewModels;

public class UserUpdateRoleViewModel
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Role { get; set; }
}