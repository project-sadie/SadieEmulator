using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Unsorted.Game.Rooms;

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

    public Room Create(int id,
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
        RoomPaintSettings paintSettings)
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
            furnitureItemRepository,
            paintSettings);
    }
}