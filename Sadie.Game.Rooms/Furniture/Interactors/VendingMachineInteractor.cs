using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class VendingMachineInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "vending_machine";
    
    public async Task OnClickAsync(RoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        item.MetaData = "1";

        var direction = RoomTileMapHelpers.GetOppositeDirection((int) item.Direction);

        roomUser.Direction = direction;
        roomUser.DirectionHead = direction;
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUser.Id,
            ItemId = new Random().Next(2, 9)
        });

        var itemWriter = new RoomFloorItemUpdatedWriter
        {
            Id = item.Id,
            AssetId = item.FurnitureItem.AssetId,
            PositionX = item.PositionX,
            PositionY = item.PositionY,
            Direction = (int)item.Direction,
            PositionZ = item.PositionZ,
            StackHeight = 0,
            Extra = 1,
            ObjectDataKey = (int)ObjectDataKey.MapKey,
            ObjectData = new Dictionary<string, string>(),
            MetaData = item.MetaData,
            Expires = -1,
            InteractionModes = 0,
            OwnerId = item.OwnerId
        };
        
        await room.UserRepository.BroadcastDataAsync(itemWriter);
    }
}