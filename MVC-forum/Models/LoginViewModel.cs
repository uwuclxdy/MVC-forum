using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models;

public class LoginViewModel
{
    [Required]
    public required string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    public bool SaveLogin { get; set; }
}
