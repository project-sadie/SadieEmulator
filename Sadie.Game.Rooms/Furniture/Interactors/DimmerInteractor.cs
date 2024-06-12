using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DimmerInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "dimmer";
    
    public async Task OnClickAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
    }

    public async Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        item.MetaData = "";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
    }

    public async Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
    }
}