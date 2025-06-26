using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class RoomAdsInteractor : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.RoomAdsBg];

    public override Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        item.PlayerFurnitureItem.MetaData = "offsetZ=0;offsetY=0;offsetX=0;clickUrl=;imageUrl=;";
        return Task.CompletedTask;
    }
}