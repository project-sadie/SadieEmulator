using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public interface IRoomFactory
{
    RoomLayout CreateLayout(int id,
        string name,
        string heightmap,
        HPoint doorPoint,
        HDirection doorDirection,
        List<RoomTile> tiles);
    
    IRoom Create(int id,
        string name,
        RoomLayout layout,
        int ownerId,
        string ownerUsername,
        string description,
        int score,
        List<string> tags,
        int maxUsersAllowed,
        RoomSettings settings,
        List<long> playersWithRights,
        IRoomFurnitureItemRepository furnitureItemRepository,
        RoomPaintSettings paintSettings);
}