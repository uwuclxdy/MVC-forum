using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models;

public class AddArticleViewModel
{
    [Required]
    public required string Title { get; init; }

    [Required]
    public required string Content { get; init; }

    [Display(Name = "Img")]
    public IFormFile? Img { get; init; }
}
