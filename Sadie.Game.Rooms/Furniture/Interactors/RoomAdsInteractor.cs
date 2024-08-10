using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums;
using Sadie.Enums.Unsorted;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class RoomAdsInteractor : AbstractRoomFurnitureItemInteractor
{
    public override string InteractionType => FurnitureItemInteractionType.RoomAdsBg;

    public override Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        item.PlayerFurnitureItem.MetaData = "offsetZ=0;offsetY=0;offsetX=0;clickUrl=;imageUrl=;";
        return Task.CompletedTask;
    }
}