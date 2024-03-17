using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomTile(int x, int y, float z, RoomTileState state)
{
    public HPoint Point { get; } = new(x, y, z);
    public RoomTileState State { get; } = state;
}