namespace Sadie.Game.Players.Room;

public class PlayerRoomVisit
{
    public PlayerRoomVisit(int playerId,
        int roomId)
    {
        PlayerId = playerId;
        RoomId = roomId;
        CreatedAt = DateTime.Now;
    }

    public int PlayerId { get; }
    public int RoomId { get; }
    public DateTime CreatedAt { get; }
}