using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_forum.Data;
using MVC_forum.Models.Entities;

namespace MVC_forum.Controllers;

public class CommentsController(ApplicationDbContext dbContext, UserManager<User> userManager) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Create(string articleId, string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return BadRequest();
        }

        var article = await dbContext.Articles.FindAsync(Guid.Parse(articleId));
        if (article == null)
        {
            return NotFound();
        }

        var user = await userManager.GetUserAsync(User);

        if (user != null)
        {
            var comment = new Comment
            {
                Content = content,
                CreatedAt = DateTime.UtcNow,
                ArticleId = Guid.Parse(articleId),
                User = user
            };

            dbContext.Comments.Add(comment);
        }

        await dbContext.SaveChangesAsync();

        return RedirectToAction("Article", "Articles", new { id = articleId });
    }
}
