using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models;

public class RegisterViewModel
{
    [Required]
    [StringLength(30, ErrorMessage = "Uporabniško ime lahko ima največ 30 znakov", MinimumLength = 3)]
    public required string Username { get; set; }

    [Required]
    [StringLength(25, ErrorMessage = "Geslo mora imeti najmanj 6 in največ 25 znakov", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Gesli se morata ujemati")]
    public required string ConfirmPassword { get; set; }

    public bool SaveLogin { get; set; }
}
