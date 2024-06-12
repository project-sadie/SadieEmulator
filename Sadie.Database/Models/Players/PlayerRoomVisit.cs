namespace Sadie.Database.Models.Players;

public class PlayerRoomVisit
{
    public required int PlayerId { get; init; }
    public required int RoomId { get; init; }
    public required DateTime CreatedAt { get; init; }
}