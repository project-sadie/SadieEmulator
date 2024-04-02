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
    
    IRoomSettings CreateSettings(
        bool walkDiagonal, 
        RoomAccessType accessType, 
        string password,  
        int whoCanMute, 
        int whoCanKick, 
        int whoCanBan, 
        bool allowPets, 
        bool canPetsEat, 
        bool hideWalls, 
        int wallThickness, 
        int floorThickness, 
        bool canUsersOverlap,
        int chatType, 
        int chatWeight, 
        int chatSpeed, 
        int chatDistance, 
        int chatProtection, 
        int tradeOption);
    
    IRoom Create(int id,
        string name,
        RoomLayout layout,
        int ownerId,
        string ownerUsername,
        string description,
        int score,
        List<string> tags,
        int maxUsersAllowed,
        IRoomSettings settings,
        List<long> playersWithRights,
        IRoomFurnitureItemRepository furnitureItemRepository,
        RoomPaintSettings paintSettings);
}