using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class GenericWiredEffectInteractor(IRoomWiredService wiredService) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [
        FurnitureItemInteractionType.WiredEffectShowMessage,
        FurnitureItemInteractionType.WiredEffectKickUser
    ];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredEffectWriter
        {
            Item = item.PlayerFurnitureItem!,
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = 0,
            SelectedItems = [],
            WiredEffectType = wiredService.GetWiredCode(item.FurnitureItem!.InteractionType),
        });
    }
}