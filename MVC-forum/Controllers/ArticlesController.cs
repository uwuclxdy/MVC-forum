using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_forum.Data;
using MVC_forum.Models;
using MVC_forum.Models.Entities;

namespace MVC_forum.Controllers
{
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

        public IActionResult Article(Guid id)
        {
            var article = dbContext.Articles
                .Include(c => c.Comments)
                .ThenInclude(c => c.User)
                .FirstOrDefault(c => c.Id == id);

            if (article == null) return NotFound();

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
                AuthorPFP = user.PFPDir,
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
            var results = dbContext.Articles
                .Where(c => c.Title.ToLower().Contains(lowerQuery) || c.Content.ToLower().Contains(lowerQuery))
                .ToList();

            var model = new SearchViewModel
            {
                Query = query,
                Results = results
            };

            return View(model);
        }

    }
}
