using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Furniture;

public class RoomFurnitureItemHelpers
{
    public static async Task CycleInteractionStateForItemAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem,
        SadieContext dbContext)
    {
        if (string.IsNullOrEmpty(roomFurnitureItem.PlayerFurnitureItem.MetaData))
        {
            roomFurnitureItem.PlayerFurnitureItem.MetaData = 0.ToString();
        }

        if (roomFurnitureItem.FurnitureItem.InteractionModes < 1 || !int.TryParse(roomFurnitureItem.PlayerFurnitureItem.MetaData, out var state))
        {
            return;
        }

        if (state >= roomFurnitureItem.FurnitureItem.InteractionModes)
        {
            state = 0;
        }

        await UpdateMetaDataForItemAsync(room, roomFurnitureItem, (state + 1).ToString());
        
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem!).Property(x => x.MetaData).IsModified = true;
        await dbContext.SaveChangesAsync();
    }

    public static async Task UpdateMetaDataForItemAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem, 
        string metaData)
    {
        roomFurnitureItem.PlayerFurnitureItem.MetaData = metaData;
        await BroadcastItemUpdateToRoomAsync(room, roomFurnitureItem);
    }

    private static async Task BroadcastItemUpdateToRoomAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem)
    {
        AbstractPacketWriter itemWriter = roomFurnitureItem.FurnitureItem.Type == FurnitureItemType.Floor ? new RoomFloorItemUpdatedWriter
            {
                Id = roomFurnitureItem.PlayerFurnitureItemId,
                AssetId = roomFurnitureItem.FurnitureItem.AssetId,
                PositionX = roomFurnitureItem.PositionX,
                PositionY = roomFurnitureItem.PositionY,
                Direction = (int)roomFurnitureItem.Direction,
                PositionZ = roomFurnitureItem.PositionZ,
                StackHeight = 0.ToString(),
                Extra = 0,
                ObjectDataKey = (int) GetObjectDataKeyForItem(roomFurnitureItem),
                ObjectData = GetObjectDataForItem(roomFurnitureItem),
                MetaData = roomFurnitureItem.PlayerFurnitureItem.MetaData,
                Expires = -1,
                InteractionModes = 1,
                OwnerId = roomFurnitureItem.PlayerFurnitureItem.PlayerId,
            }
            : new RoomWallFurnitureItemUpdatedWriter
        {
            Item = roomFurnitureItem
        };
        
        await room.UserRepository.BroadcastDataAsync(itemWriter);
    }

    public static ObjectDataKey GetObjectDataKeyForItem(PlayerFurnitureItemPlacementData furnitureItem)
    {
        if (furnitureItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.RoomAdsBg)
        {
            return ObjectDataKey.MapKey;
        }
        
        return ObjectDataKey.LegacyKey;
    }

    public static Dictionary<string, string> GetObjectDataForItem(PlayerFurnitureItemPlacementData furnitureItem)
    {
        if (furnitureItem.FurnitureItem!.InteractionType == FurnitureItemInteractionType.RoomAdsBg)
        {
            var data = new Dictionary<string, string>();
            
            foreach (var piece in furnitureItem.PlayerFurnitureItem.MetaData.Split(";"))
            {
                var parts = piece.Split("=");
                var key = parts[0];
                var value = parts.Length < 2 ? "" : parts[1];

                data[key] = value;
            }

            return data;
        }

        return new Dictionary<string, string>();
    }
}