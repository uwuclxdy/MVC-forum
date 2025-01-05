using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_forum.Data;
using MVC_forum.Models;
using MVC_forum.Models.Entities;

namespace MVC_forum.Controllers;

public class ArticlesController(ApplicationDbContext dbContext, UserManager<User> userManager) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var articles = dbContext.Articles
            .OrderByDescending(c => c.Date)
            .ToList();

        return View(articles);
    }

    [HttpGet]
    public IActionResult Article(Guid id)
    {
        var article = dbContext.Articles
            .Include(c => c.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefault(c => c.Id == id);

        if (article == null) return NotFound();

        var author = dbContext.Users.FirstOrDefault(u => u.UserName == article.Author);
        if (author == null || author.PfpDir == article.AuthorPfp) return View(article);

        article.AuthorPfp = author.PfpDir;
        dbContext.SaveChanges();

        return View(article);
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddArticleViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var article = new Article
        {
            Title = viewModel.Title,
            Content = viewModel.Content,
            Author = user.UserName!,
            AuthorPfp = user.PfpDir,
            Date = DateTime.UtcNow
        };

        await dbContext.Articles.AddAsync(article);
        await dbContext.SaveChangesAsync();

        if (viewModel.Img == null) return RedirectToAction("Index");

        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "slike", $"{article.Id}.jpg");
        await using (var stream = new FileStream(imagePath, FileMode.Create))
        {
            await viewModel.Img.CopyToAsync(stream);
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return View(new SearchViewModel { Query = query, Results = [] });
        }

        var lowerQuery = query.ToLower();
        var articles = dbContext.Articles.ToList();
        var results = articles
            .Where(c => c.Title.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase) || c.Content.Contains(lowerQuery, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var model = new SearchViewModel
        {
            Query = query,
            Results = results
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> LikeArticle(Guid id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var article = await dbContext.Articles.FindAsync(id);
        if (article == null) return NotFound();

        var likeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == article.Id && ui.InteractionType_ == UserInteraction.InteractionType.Like);

        var dislikeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == article.Id && ui.InteractionType_ == UserInteraction.InteractionType.Dislike);

        if (likeInteraction == null)
        {
            if (dislikeInteraction != null)
            {
                dbContext.UserInteractions.Remove(dislikeInteraction);
                article.Dislikes--;
            }

            dbContext.UserInteractions.Add(new UserInteraction
            {
                UserId = user.Id,
                TargetId = article.Id,
                InteractionType_ = UserInteraction.InteractionType.Like,
                User = user
            });
            article.Likes++;
        }
        else
        {
            dbContext.UserInteractions.Remove(likeInteraction);
            article.Likes--;
        }

        await dbContext.SaveChangesAsync();

        return RedirectToAction("Article", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> DislikeArticle(Guid id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var article = await dbContext.Articles.FindAsync(id);
        if (article == null) return NotFound();

        var likeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == article.Id && ui.InteractionType_ == UserInteraction.InteractionType.Like);

        var dislikeInteraction = await dbContext.UserInteractions
            .FirstOrDefaultAsync(ui => ui.UserId == user.Id && ui.TargetId == article.Id && ui.InteractionType_ == UserInteraction.InteractionType.Dislike);

        if (dislikeInteraction == null)
        {
            if (likeInteraction != null)
            {
                dbContext.UserInteractions.Remove(likeInteraction);
                article.Likes--;
            }

            dbContext.UserInteractions.Add(new UserInteraction
            {
                UserId = user.Id,
                TargetId = article.Id,
                InteractionType_ = UserInteraction.InteractionType.Dislike,
                User = user
            });
            article.Dislikes++;
        }
        else
        {
            dbContext.UserInteractions.Remove(dislikeInteraction);
            article.Dislikes--;
        }

        await dbContext.SaveChangesAsync();

        return RedirectToAction("Article", new { id });
    }
}
