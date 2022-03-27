namespace Sadie.Game.Rooms;

public interface IRoomSettings
{
    bool CanWalkDiagonal { get; }
    bool Muted { get; }
}