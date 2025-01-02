using System.Collections;
using Microsoft.AspNetCore.Identity;

namespace MVC_forum.Models.Entities;

public class User : IdentityUser
{
    public string PFPDir { get; set; }
    public List<Article> Articles { get; set; }
}
