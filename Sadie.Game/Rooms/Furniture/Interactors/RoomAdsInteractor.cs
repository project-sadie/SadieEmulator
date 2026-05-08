using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class RoomAdsInteractor : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.RoomAdsBg];

    public override Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementDataDto item, IRoomUser roomUser)
    {
        item.PlayerFurnitureItem.MetaData = "offsetZ=0;offsetY=0;offsetX=0;clickUrl=;imageUrl=;";
        return Task.CompletedTask;
    }
}