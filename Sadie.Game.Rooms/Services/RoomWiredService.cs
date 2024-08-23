using Sadie.Database.Models.Players.Furniture;

namespace Sadie.Game.Rooms.Services;

public class RoomWiredService : IRoomWiredService
{
    public async Task RunTriggerAsync(PlayerFurnitureItemPlacementData trigger,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems)
    {
        var effectsOnTrigger = roomItems
            .Where(x =>
                x.PositionX == trigger.PositionX &&
                x.PositionY == trigger.PositionY &&
                x.PositionZ >= trigger.PositionZ);

        foreach (var effect in effectsOnTrigger)
        {
            await RunEffectAsync(effect);
        }
    }

    private async Task RunEffectAsync(PlayerFurnitureItemPlacementData effect)
    {
        switch (effect.FurnitureItem.InteractionType)
        {
            case "wf_act_show_message":
                
                break;
        }
    }
}