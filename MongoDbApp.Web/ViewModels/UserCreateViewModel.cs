using System.ComponentModel.DataAnnotations;

namespace MongoDbApp.Web.ViewModels;

public class UserCreateViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Role { get; set; }
}