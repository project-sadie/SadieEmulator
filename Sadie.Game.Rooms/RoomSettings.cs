namespace Sadie.Game.Rooms;

public class RoomSettings
{
    protected RoomSettings(bool useDiagonal)
    {
        UseDiagonal = useDiagonal;
    }

    public bool UseDiagonal { get; }
}