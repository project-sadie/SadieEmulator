using Sadie.Shared;

namespace Sadie.Game.Rooms;

public class RoomTile
{
    public HPoint Point { get; }
    public RoomTileState State { get; }
    
    public RoomTile(int x, int y, float z, RoomTileState state)
    {
        Point = new HPoint(x, y, z);
        State = state;
    }
}