using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Db.Models.Constants;
using Sadie.Networking.Packets.Writers.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class GenericWiredTriggerInteractor(IRoomWiredService wiredService,
    ServerRoomConstants roomConstants) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes =>
    [
        FurnitureItemInteractionType.WiredTriggerSaysSomething,
        FurnitureItemInteractionType.WiredTriggerEnterRoom
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementDataDto item, IRoomUser roomUser)
    {
        var wiredData = item.WiredData;
        
        var selectedItemIds = wiredData?
            .SelectedItems
            .Select(x => x.Id)
            .ToList() ?? [];

        var input = wiredData?.Message ?? "";
        
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredTriggerWriter
        {
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = roomConstants.WiredMaxFurnitureSelection,
            SelectedItemIds = selectedItemIds,
            AssetId = 0,
            Id = 0,
            Input = input,
            IntParameters = [],
            StuffTypeSelectionCode = 0,
            TriggerConfig = wiredService.GetWiredCode(item.PlayerFurnitureItem.FurnitureItem.InteractionType),
            ConflictingEffectIds = []
        });
    }
}