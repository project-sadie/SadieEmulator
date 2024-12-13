namespace Sadie.Database.Models.Players;

public class PlayerBan
{
    public int Id { get; init; }
    public required int OwnerId { get; init; }
    public required Player Owner { get; init; }
    public required int PlayerId { get; init; }
    public required Player Player { get; init; }
    public required string Reason { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
}