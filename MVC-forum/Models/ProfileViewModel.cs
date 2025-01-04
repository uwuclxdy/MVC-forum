using MVC_forum.Models.Entities;

namespace MVC_forum.Models;

public class ProfileViewModel
{
    public required User User { get; init; }
    public ChangePassViewModel ChangePassViewModel { get; init; } = null!;
    public int NumberOfArticles => User.Articles!.Count;
}
