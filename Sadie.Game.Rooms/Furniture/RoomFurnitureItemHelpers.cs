using Sadie.API.Game.Rooms;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Furniture;

public class RoomFurnitureItemHelpers
{
    public static async Task CycleInteractionStateForItemAsync(
        IRoomLogic room, 
        RoomFurnitureItem roomFurnitureItem)
    {
        if (string.IsNullOrEmpty(roomFurnitureItem.MetaData))
        {
            roomFurnitureItem.MetaData = 0.ToString();
        }

        if (roomFurnitureItem.FurnitureItem.InteractionModes < 1 || !int.TryParse(roomFurnitureItem.MetaData, out var state))
        {
            return;
        }

        if (state >= roomFurnitureItem.FurnitureItem.InteractionModes)
        {
            state = 0;
        }
        
        roomFurnitureItem.MetaData = (state + 1).ToString();

        await BroadcastItemUpdateToRoomAsync(room, roomFurnitureItem);
    }

    public static async Task BroadcastItemUpdateToRoomAsync(
        IRoomLogic room, 
        RoomFurnitureItem roomFurnitureItem)
    {
        var itemWriter = new RoomFloorItemUpdatedWriter
        {
            Id = roomFurnitureItem.Id,
            AssetId = roomFurnitureItem.FurnitureItem.AssetId,
            PositionX = roomFurnitureItem.PositionX,
            PositionY = roomFurnitureItem.PositionY,
            Direction = (int)roomFurnitureItem.Direction,
            PositionZ = roomFurnitureItem.PositionZ,
            StackHeight = 0.ToString(),
            Extra = 0,
            ObjectDataKey = (int)ObjectDataKey.LegacyKey,
            ExtraData = roomFurnitureItem.MetaData,
            Expires = -1,
            InteractionModes = 1,
            OwnerId = roomFurnitureItem.OwnerId,
        };
        
        await room.UserRepository.BroadcastDataAsync(itemWriter);
    }
}