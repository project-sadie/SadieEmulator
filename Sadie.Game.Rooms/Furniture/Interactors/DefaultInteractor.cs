using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DefaultInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "default";
    
    public async Task OnClickAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        if (string.IsNullOrEmpty(item.MetaData))
        {
            item.MetaData = 0.ToString();
        }

        if (item.FurnitureItem.InteractionModes < 1 || !int.TryParse(item.MetaData, out var state))
        {
            return;
        }

        if (state > item.FurnitureItem.InteractionModes)
        {
            state = 0;
        }
        
        item.MetaData = (state + 1).ToString();

        var itemWriter = new RoomFloorItemUpdatedWriter
        {
            Id = item.Id,
            AssetId = item.FurnitureItem.AssetId,
            PositionX = item.PositionX,
            PositionY = item.PositionY,
            Direction = (int)item.Direction,
            PositionZ = item.PositionZ,
            StackHeight = 0.ToString(),
            Extra = 0,
            ObjectDataKey = (int)ObjectDataKey.LegacyKey,
            ExtraData = item.MetaData,
            Expires = -1,
            InteractionModes = 1,
            OwnerId = item.OwnerId,
        };
        
        await room.UserRepository.BroadcastDataAsync(itemWriter);
    }
}