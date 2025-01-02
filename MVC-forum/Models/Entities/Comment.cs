namespace MVC_forum.Models.Entities;

public class Comment
{
    public Guid Id { get; set; }

    public required Guid ArticleId { get; set; }

    public required string Content { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required string UserId { get; set; }
    public User? User { get; set; }
}
