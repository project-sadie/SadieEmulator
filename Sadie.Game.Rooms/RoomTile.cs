using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomTile(int x, int y, float z, RoomTileState state, List<RoomFurnitureItem> items)
{
    public HPoint Point { get; } = new(x, y, z);
    public RoomTileState State { get; } = state;
    public List<RoomFurnitureItem> Items { get; } = items;
    public Dictionary<long, IRoomUser> Users { get; } = [];
}