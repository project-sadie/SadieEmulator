namespace Sadie.Database.Models.Players;

public class PlayerRoomLike
{
    public int Id { get; init; }
    public int PlayerId { get; init; }
    public int RoomId { get; init; }
}