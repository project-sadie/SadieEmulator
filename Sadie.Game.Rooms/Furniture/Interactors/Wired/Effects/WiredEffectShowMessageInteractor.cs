using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired.Effects;

public class WiredEffectShowMessageInteractor(IRoomWiredService wiredService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [
        FurnitureItemInteractionType.WiredEffectShowMessage
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var wiredData = item.WiredData;
        var input = wiredData?.Message ?? "";
        
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredMessageEffectWriter
        {
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = 0,
            SelectedItemIds = [],
            WiredEffectType = item.FurnitureItem.AssetId,
            Id = item.PlayerFurnitureItemId,
            Input = input,
            IntParams = [],
            StuffTypeSelectionCode = 0,
            Type = wiredService.GetTriggerCodeFromInteractionType(item.FurnitureItem.InteractionType),
            DelayInPulses = 0,
            ConflictingTriggerIds = []
        });
    }
}