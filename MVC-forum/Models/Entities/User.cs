using Microsoft.AspNetCore.Identity;

namespace MVC_forum.Models.Entities;

public class User : IdentityUser
{
    public string PFPDir { get; set; }
    public List<Article> Articles { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime LastLogin { get; set; }
    public int NumberOfComments { get; set; }
}
