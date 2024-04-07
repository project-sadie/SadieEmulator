namespace Sadie.Database.Models.Players;

public class PlayerRoomLike
{
    public int Id { get; set; }
    public long PlayerId { get; set; }
    public int RoomId { get; set; }
}