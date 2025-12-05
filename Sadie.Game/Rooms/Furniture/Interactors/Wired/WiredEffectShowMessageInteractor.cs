using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Networking.Packets.Writers.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class WiredEffectShowMessageInteractor(IRoomWiredService wiredService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [
        FurnitureItemInteractionType.WiredEffectShowMessage
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementDataDto item, IRoomUser roomUser)
    {
        var wiredData = item.WiredData;
        var input = wiredData?.Message ?? "";
        var furnitureItem = item.PlayerFurnitureItem.FurnitureItem;
        
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredMessageEffectWriter
        {
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = 0,
            SelectedItemIds = [],
            WiredEffectType = furnitureItem.AssetId,
            Id = item.Id,
            Input = input,
            IntParams = [],
            StuffTypeSelectionCode = 0,
            Type = wiredService.GetWiredCode(furnitureItem.InteractionType ?? ""),
            DelayInPulses = 0,
            ConflictingTriggerIds = []
        });
    }
}