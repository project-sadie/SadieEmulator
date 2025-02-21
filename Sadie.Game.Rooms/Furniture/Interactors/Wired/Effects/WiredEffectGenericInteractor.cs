using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired.Effects;

public class WiredEffectGenericInteractor(IRoomWiredService wiredService, ServerRoomConstants roomConstants) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [
        FurnitureItemInteractionType.WiredEffectKickUser,
        FurnitureItemInteractionType.WiredEffectShowMessage,
        FurnitureItemInteractionType.WiredEffectTeleportToFurniture,
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var wiredData = item.WiredData;
        var input = wiredData?.Message ?? "";

        var placementDataIds = wiredData?
            .PlayerFurnitureItemWiredItems
            .Select(x => x.PlayerFurnitureItemPlacementDataId) ?? [];

        var selectedItemIds = room
            .FurnitureItems
            .Where(x => placementDataIds.Contains(x.Id))
            .Select(x => x.PlayerFurnitureItemId)
            .ToList();
        
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredMessageEffectWriter
        {
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = roomConstants.WiredMaxFurnitureSelection,
            SelectedItemIds = selectedItemIds,
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