using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_forum.Data;
using MVC_forum.Models;

namespace MVC_forum.Controllers;

public class HomeController(ApplicationDbContext dbContext) : Controller
{
    public IActionResult Index()
    {
        var latestClanki = dbContext.Articles
            .OrderByDescending(c => c.Date)
            .Take(3)
            .ToList();

        return View(latestClanki);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
