using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class WiredTriggerUserWalksOnInteractor(IRoomWiredService wiredService,
    ServerRoomConstants roomConstants) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes =>
    [
        FurnitureItemInteractionType.WiredTriggerUserWalksOnFurniture
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var wiredData = item.WiredData;
        
        var selectedItemIds = wiredData?
            .SelectedItems
            .Select(x => x.Id)
            .ToList() ?? [];
        
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredTriggerWriter
        {
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = roomConstants.WiredMaxFurnitureSelection,
            SelectedItemIds = selectedItemIds,
            AssetId = item.FurnitureItem.AssetId,
            Id = item.Id,
            IntParameters = [],
            StuffTypeSelectionCode = 0,
            TriggerConfig = wiredService.GetWiredCode(item.FurnitureItem.InteractionType),
            ConflictingEffectIds = []
        });
    }
}