using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Game.Rooms.Wired.Effects.Actions;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Wired.Effects.Actions;

public class KickUserEffectAction : IWiredEffectAction
{
    public string InteractionType => FurnitureItemInteractionType.WiredEffectShowMessage;

    public async Task ExecuteAsync(
        IRoomLogic room,
        IRoomUser userWhoTriggered,
        PlayerFurnitureItemPlacementData effect)
    {
        if (room.OwnerId == userWhoTriggered.Id)
        {
            return;
        }
                
        await userWhoTriggered
            .Room
            .UserRepository
            .TryRemoveAsync(userWhoTriggered.Id, true, true);

        var message = effect.WiredData?.Message ?? string.Empty;

        if (!string.IsNullOrEmpty(message))
        {
            await userWhoTriggered.Player.SendAlertAsync(message);
        }
    }
}