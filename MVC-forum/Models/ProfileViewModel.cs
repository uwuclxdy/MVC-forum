using MVC_forum.Models.Entities;

namespace MVC_forum.Models;

public class ProfileViewModel
{
    public required User? User { get; set; }
    public ChangePassViewModel? ChangePassViewModel { get; set; }
}
