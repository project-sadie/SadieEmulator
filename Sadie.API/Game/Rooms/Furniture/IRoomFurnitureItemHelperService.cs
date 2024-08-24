using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Unsorted;

namespace Sadie.Game.Rooms.Furniture;

public interface IRoomFurnitureItemHelperService
{
    Task CycleInteractionStateForItemAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem,
        SadieContext dbContext);

    Task UpdateMetaDataForItemAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem, 
        string metaData);

    Task BroadcastItemUpdateToRoomAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem);

    ObjectDataKey GetObjectDataKeyForItem(PlayerFurnitureItemPlacementData furnitureItem);
    Dictionary<string, string> GetObjectDataForItem(PlayerFurnitureItemPlacementData furnitureItem);
}