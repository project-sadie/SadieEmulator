namespace Sadie.Game.Players.Room;

public class PlayerRoomVisit(
    int playerId,
    int roomId)
{
    public int PlayerId { get; } = playerId;
    public int RoomId { get; } = roomId;
    public DateTime CreatedAt { get; } = DateTime.Now;
}