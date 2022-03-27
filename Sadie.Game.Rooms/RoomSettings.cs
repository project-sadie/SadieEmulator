namespace Sadie.Game.Rooms;

public class RoomSettings : IRoomSettings
{
    public RoomSettings(bool walkDiagonal, bool muted)
    {
        CanWalkDiagonal = walkDiagonal;
        Muted = muted;
    }

    public bool CanWalkDiagonal { get; }
    public bool Muted { get; }
}