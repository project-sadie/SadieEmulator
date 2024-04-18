namespace Sadie.Database.Models.Rooms.Rights;

public class RoomPlayerRight
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int PlayerId { get; set; }
}