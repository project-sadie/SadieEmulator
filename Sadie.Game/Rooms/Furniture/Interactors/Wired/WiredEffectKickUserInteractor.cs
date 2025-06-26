using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class WiredEffectKickUserInteractor(IRoomWiredService wiredService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [
        FurnitureItemInteractionType.WiredEffectKickUser
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var wiredData = item.WiredData;
        var input = wiredData?.Message ?? "";
        
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredMessageEffectWriter
        {
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = 5,
            SelectedItemIds = [],
            WiredEffectType = item.FurnitureItem.AssetId,
            Id = item.Id,
            Input = input,
            IntParams = [],
            StuffTypeSelectionCode = 0,
            Type = wiredService.GetWiredCode(item.FurnitureItem.InteractionType),
            DelayInPulses = 0,
            ConflictingTriggerIds = []
        });
    }
}