using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models;

public class LoginViewModel
{
    [Required]
    public required string Username { get; init; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; init; }

    public bool SaveLogin { get; init; }
}
