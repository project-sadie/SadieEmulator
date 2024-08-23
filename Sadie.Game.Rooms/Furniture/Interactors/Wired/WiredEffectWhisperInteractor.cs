using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors.Wired;

public class WiredEffectWhisperInteractor(ServerRoomConstants roomConstants) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => ["wf_act_show_message"];

    public override async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
    {
        await roomUser.NetworkObject.WriteToStreamAsync(new WiredEffectWriter
        {
            Item = item.PlayerFurnitureItem!,
            StuffTypeSelectionEnabled = false,
            MaxItemsSelected = 0,
            SelectedItems = []
        });
    }
}