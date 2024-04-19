namespace Sadie.Database.Models.Rooms.Rights;

public class RoomPlayerRight
{
    public int Id { get; init; }
    public int RoomId { get; init; }
    public int PlayerId { get; init; }
}