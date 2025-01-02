namespace MVC_forum.Models.Entities;

public class Article
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public string Img { get; set; } = "";

    public required string Content { get; set; }

    public required string Author { get; set; }

    public required string AuthorPFP { get; set; }

    public required DateTime Date { get; set; }

    public List<Comment> Comments { get; set; } = [];
}
