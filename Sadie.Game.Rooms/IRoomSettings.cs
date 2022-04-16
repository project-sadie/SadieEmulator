namespace Sadie.Game.Rooms;

public interface IRoomSettings
{
    bool CanWalkDiagonal { get; }
    RoomAccessType AccessType { get; }
    string Password { get; }
}