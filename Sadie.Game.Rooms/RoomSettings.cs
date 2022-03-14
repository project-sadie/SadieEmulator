namespace Sadie.Game.Rooms;

public class RoomSettings
{
    public RoomSettings(bool useDiagonal)
    {
        UseDiagonal = useDiagonal;
    }

    public bool UseDiagonal { get; }
}