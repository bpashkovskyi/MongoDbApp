using System.ComponentModel.DataAnnotations;

namespace MongoDbApp.ViewModels;

public class UserUpdateViewModel
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Role { get; set; }
}