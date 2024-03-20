using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomFactory(IServiceProvider serviceProvider) : IRoomFactory
{
    public RoomLayout CreateLayout(int id,
        string name,
        string heightmap,
        HPoint doorPoint,
        HDirection doorDirection,
        List<RoomTile> tiles)
    {
        return ActivatorUtilities.CreateInstance<RoomLayout>(
            serviceProvider, 
            id, 
            name, 
            heightmap, 
            doorPoint, 
            doorDirection,
            tiles);
    }

    public IRoomSettings CreateSettings(
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
        int tradeOption)
    {
        return ActivatorUtilities.CreateInstance<RoomSettings>(
            serviceProvider, 
            walkDiagonal, 
            accessType, 
            password, 
            whoCanMute, 
            whoCanKick, 
            whoCanBan, 
            allowPets, 
            canPetsEat, 
            hideWalls, 
            wallThickness,
            floorThickness,
            canUsersOverlap,
            chatType, 
            chatWeight, 
            chatSpeed,
            chatDistance, 
            chatProtection, 
            tradeOption);
    }

    public IRoom Create(int id,
        string name,
        RoomLayout layout,
        int ownerId,
        string ownerUsername,
        string description,
        int score,
        List<string> tags,
        int maxUsersAllowed,
        IRoomSettings settings,
        List<int> playersWithRights,
        IRoomFurnitureItemRepository furnitureItemRepository)
    {
        return ActivatorUtilities.CreateInstance<Room>(
            serviceProvider,
            id, 
            name,
            layout,
            ownerId,
            ownerUsername,
            description,
            score,
            tags,
            maxUsersAllowed,
            settings,
            playersWithRights,
            furnitureItemRepository);
    }
}