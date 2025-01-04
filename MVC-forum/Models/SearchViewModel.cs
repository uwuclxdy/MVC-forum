using MVC_forum.Models.Entities;

namespace MVC_forum.Models;

public class SearchViewModel
{
    public string Query { get; init; } = "";
    public List<Article> Results { get; init; } = [];
}
