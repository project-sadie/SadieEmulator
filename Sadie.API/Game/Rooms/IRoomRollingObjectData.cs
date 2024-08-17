namespace Sadie.API.Game.Rooms;

public interface IRoomRollingObjectData
{
    int Id { init; get; }
    string Height { init; get; }
    string NextHeight { init; get; }
}