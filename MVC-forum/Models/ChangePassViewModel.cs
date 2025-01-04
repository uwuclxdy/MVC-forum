using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models;

public class ChangePassViewModel
{
    [Required]
    [DataType(DataType.Password)]
    public string? OldPassword { get; init; }

    [Required]
    [StringLength(100, ErrorMessage = "The password must be at least 6 characters long, must contain a number and a capital letter.", MinimumLength = 6)]
    [DataType(DataType.Password, ErrorMessage = "The password must be at least 6 characters long, must contain a number and a capital letter.")]
    public string? NewPassword { get; init; }

    [DataType(DataType.Password)]
    [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
    public string? ConfirmPassword { get; init; }
}
