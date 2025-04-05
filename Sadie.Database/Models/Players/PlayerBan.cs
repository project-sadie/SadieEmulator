namespace Sadie.Database.Models.Players;

public class PlayerBan
{
    public int Id { get; init; }
    public required long CreatorId { get; init; }
    public required Player Creator { get; init; }
    public required long PlayerId { get; init; }
    public required Player Player { get; init; }
    public required string Reason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}