using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class VendingMachineInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "vending_machine";
    
    public async Task OnClickAsync(RoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        item.MetaData = "1";

        var direction = RoomHelpers.GetOppositeDirection((int) item.Direction);

        roomUser.Direction = direction;
        roomUser.DirectionHead = direction;
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter(roomUser.Id, new Random().Next(2, 9)));
        await room.UserRepository.BroadcastDataAsync(new RoomFloorItemUpdatedWriter(
            item.Id,
            item.FurnitureItem.AssetId,
            item.PositionX,
            item.PositionY,
            item.PositionZ,
            (int) item.Direction,
            0,
            1,
            (int) ObjectDataKey.MapKey,
            new Dictionary<string, string>(),
            item.FurnitureItem.InteractionType,
            item.MetaData,
            item.FurnitureItem.InteractionModes,
            -1,
            item.OwnerId));
    }
}