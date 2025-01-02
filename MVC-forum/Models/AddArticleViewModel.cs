using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models;

public class AddArticleViewModel
{
    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Content { get; set; }

    [Display(Name = "Img")]
    public IFormFile? Img { get; set; }
}
