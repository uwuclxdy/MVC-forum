using System.ComponentModel.DataAnnotations;

namespace MVC_forum.Models.Entities;

public class Comment
{
    public Guid Id { get; init; }

    public required Guid ArticleId { get; init; }

    [MaxLength(1000)]
    public required string Content { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required User User { get; init; }
}
