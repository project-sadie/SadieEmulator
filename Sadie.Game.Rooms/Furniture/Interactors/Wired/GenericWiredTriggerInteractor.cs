using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class GenericWiredTriggerInteractor(IRoomWiredService wiredService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes =>
    [
        FurnitureItemInteractionType.WiredTriggerSaysSomething,
        FurnitureItemInteractionType.WiredTriggerEnterRoom,
        FurnitureItemInteractionType.WiredTriggerUserWalksOnFurniture,
        FurnitureItemInteractionType.WiredTriggerUserWalksOffFurniture,
        FurnitureItemInteractionType.WiredTriggerFurnitureStateChanged,
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredTriggerWriter
        {
            Item = item.PlayerFurnitureItem!,
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = 5,
            SelectedItems = [],
            TriggerCode = wiredService.GetWiredCode(item.FurnitureItem!.InteractionType),
        });
    }
}