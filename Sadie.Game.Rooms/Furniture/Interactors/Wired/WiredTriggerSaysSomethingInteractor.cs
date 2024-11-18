using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class WiredTriggerSaysSomethingInteractor(ServerRoomConstants roomConstants) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [
        FurnitureItemInteractionType.WiredTriggerSaysSomething
    ];

    public override async Task OnTriggerAsync(IRoomLogic room,
        PlayerFurnitureItemPlacementData item,
        IRoomUser roomUser)
    {
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredTriggerWriter
        {
            Item = item.PlayerFurnitureItem!,
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = 12, // TODO; Store this somewhere
            SelectedItems = []
        });
    }
}