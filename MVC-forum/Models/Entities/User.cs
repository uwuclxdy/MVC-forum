using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MVC_forum.Models.Entities;

public class User : IdentityUser
{
    [MaxLength(80)]
    public string? PfpDir { get; set; }
    public List<Article>? Articles { get; set; }
    public DateTime RegistrationDate { get; init; }
    public DateTime LastLogin { get; set; }
    public int NumberOfComments { get; set; }
}
