namespace MVC_forum.Models.Entities;

public class UserInteraction
{
    public Guid Id { get; init; }

    public required string UserId { get; init; }
    public required User User { get; init; }

    public required Guid TargetId { get; init; }
    public required InteractionType InteractionType_ { get; init; }

    public enum InteractionType
    {
        Like,
        Dislike
    }
}
