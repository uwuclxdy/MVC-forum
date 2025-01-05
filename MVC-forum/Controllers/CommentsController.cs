using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost]
    public async Task<IActionResult> LikeComment(Guid id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var comment = await dbContext.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        var likeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == comment.Id && ui.InteractionType_ == UserInteraction.InteractionType.Like);

        var dislikeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == comment.Id && ui.InteractionType_ == UserInteraction.InteractionType.Dislike);

        if (likeInteraction == null)
        {
            if (dislikeInteraction != null)
            {
                dbContext.UserInteractions.Remove(dislikeInteraction);
                comment.Dislikes--;
            }

            dbContext.UserInteractions.Add(new UserInteraction
            {
                UserId = user.Id,
                TargetId = comment.Id,
                InteractionType_ = UserInteraction.InteractionType.Like,
                User = user
            });
            comment.Likes++;
        }
        else
        {
            dbContext.UserInteractions.Remove(likeInteraction);
            comment.Likes--;
        }

        await dbContext.SaveChangesAsync();

        return RedirectToAction("Article", "Articles", new { id = comment.ArticleId });
    }

    [HttpPost]
    public async Task<IActionResult> DislikeComment(Guid id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var comment = await dbContext.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        var likeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == comment.Id && ui.InteractionType_ == UserInteraction.InteractionType.Like);

        var dislikeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == comment.Id && ui.InteractionType_ == UserInteraction.InteractionType.Dislike);

        if (dislikeInteraction == null)
        {
            if (likeInteraction != null)
            {
                dbContext.UserInteractions.Remove(likeInteraction);
                comment.Likes--;
            }

            dbContext.UserInteractions.Add(new UserInteraction
            {
                UserId = user.Id,
                TargetId = comment.Id,
                InteractionType_ = UserInteraction.InteractionType.Dislike,
                User = user
            });
            comment.Dislikes++;
        }
        else
        {
            dbContext.UserInteractions.Remove(dislikeInteraction);
            comment.Dislikes--;
        }

        await dbContext.SaveChangesAsync();

        return RedirectToAction("Article", "Articles", new { id = comment.ArticleId });
    }
}
