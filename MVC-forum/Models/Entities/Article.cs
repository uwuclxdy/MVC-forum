using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models.Entities;

public class Article
{
    public Guid Id { get; init; }

    [MaxLength(60)]
    public required string Title { get; init; }

    [MaxLength(100000)]
    public required string Content { get; init; }

    [MaxLength(30)]
    public required string Author { get; init; }

    [MaxLength(1000)]
    public required string? AuthorPfp { get; init; }

    public required DateTime Date { get; init; }

    public List<Comment> Comments { get; init; } = [];
}
