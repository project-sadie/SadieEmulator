using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired.Triggers;

public class WiredTriggerGenericInteractor(IRoomWiredService wiredService,
    ServerRoomConstants roomConstants,
    SadieContext dbContext) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes =>
    [
        FurnitureItemInteractionType.WiredTriggerEnterRoom,
        FurnitureItemInteractionType.WiredTriggerSaysSomething,
        FurnitureItemInteractionType.WiredTriggerUserWalksOnFurniture,
        FurnitureItemInteractionType.WiredTriggerUserWalksOffFurniture,
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        var wiredData = item.WiredData;
        var interactionType = item.FurnitureItem.InteractionType;

        var placementDataIds = wiredData?
            .PlayerFurnitureItemWiredItems
            .Select(x => x.PlayerFurnitureItemPlacementDataId) ?? [];

        var selectedItemIds = room
            .FurnitureItems
            .Where(x => placementDataIds.Contains(x.Id))
            .Select(x => x.PlayerFurnitureItemId)
            .ToList();
        
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredTriggerWriter
        {
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = roomConstants.WiredMaxFurnitureSelection,
            SelectedItemIds = selectedItemIds,
            StuffTypeId = item.FurnitureItem.AssetId,
            Id = item.PlayerFurnitureItemId,
            Input = wiredData!.Message ?? "",
            IntParameters = wiredData
                .PlayerFurnitureItemWiredParameters
                .Select(x => x.Value)
                .ToList(),
            StuffTypeSelectionCode = wiredService.GetSelectionCodeFromInteractionType(interactionType),
            TriggerConfig = wiredService.GetTriggerCodeFromInteractionType(interactionType),
            ConflictingEffectIds = []
        });
    }

    public override async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        if (item.WiredData != null)
        {
            return;
        }

        var wiredData = new PlayerFurnitureItemWiredData
        {
            PlayerFurnitureItemPlacementDataId = item.Id,
            PlacementData = item,
            Message = null,
            PlayerFurnitureItemWiredParameters = [],
            PlayerFurnitureItemWiredItems = []
        };
        
        dbContext.Entry(wiredData).State = EntityState.Added;
        dbContext.Entry(wiredData.PlacementData).State = EntityState.Unchanged;
        await dbContext.SaveChangesAsync();

        item.WiredData = wiredData;
    }
}