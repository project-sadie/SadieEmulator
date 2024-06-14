using Sadie.API.Game.Rooms;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Furniture;

public class RoomFurnitureItemHelpers
{
    public static async Task CycleInteractionStateForItemAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem)
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
        
        roomFurnitureItem.PlayerFurnitureItem.MetaData = (state + 1).ToString();

        await BroadcastItemUpdateToRoomAsync(room, roomFurnitureItem);
    }

    public static async Task BroadcastItemUpdateToRoomAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementData roomFurnitureItem)
    {
        AbstractPacketWriter itemWriter = roomFurnitureItem.FurnitureItem.Type == FurnitureItemType.Floor ? new RoomFloorItemUpdatedWriter
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
            ExtraData = roomFurnitureItem.PlayerFurnitureItem.MetaData,
            Expires = -1,
            InteractionModes = 1,
            OwnerId = roomFurnitureItem.PlayerFurnitureItem.PlayerId,
        } : new RoomWallFurnitureItemUpdatedWriter
        {
            Item = roomFurnitureItem
        };
        
        await room.UserRepository.BroadcastDataAsync(itemWriter);
    }
}