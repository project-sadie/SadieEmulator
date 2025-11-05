namespace Sadie.Db.Models.Players;

public class PlayerBan
{
    public int Id { get; init; }
    public required long CreatorId { get; init; }
    public required Player Creator { get; init; }
    public required long PlayerId { get; init; }
    public required Player Player { get; init; }
    public required string Reason { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
}