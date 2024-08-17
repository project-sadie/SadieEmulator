namespace Sadie.Game.Rooms;

public class RoomRollingObjectData : IRoomRollingObjectData
{
    public required int Id { init; get; }
    public required string Height { init; get; }
    public required string NextHeight { init; get; }
}