using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Users;

namespace Sadie.Game.Rooms.Services;

public class RoomWiredService : IRoomWiredService
{
    public IEnumerable<PlayerFurnitureItemPlacementData> GetEffectsForTrigger(
        PlayerFurnitureItemPlacementData trigger,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems)
    {
        return roomItems
            .Where(x =>
                x.PositionX == trigger.PositionX &&
                x.PositionY == trigger.PositionY &&
                x.PositionZ >= trigger.PositionZ);
    }
    
    
    
    public async Task RunTriggerForRoomAsync(IRoomLogic room,
        PlayerFurnitureItemPlacementData trigger)
    {
        var effectsOnTrigger = GetEffectsForTrigger(trigger, room.FurnitureItems);

        foreach (var effect in effectsOnTrigger)
        {
            await RunEffectForRoomAsync(room, effect);
        }
    }

    private static async Task RunEffectForRoomAsync(
        IRoomLogic room,
        PlayerFurnitureItemPlacementData effect)
    {
        switch (effect.FurnitureItem!.InteractionType)
        {
            case FurnitureItemInteractionType.WiredEffectShowMessage:
                foreach (var roomUser in room.UserRepository.GetAll())
                {
                    await roomUser.NetworkObject.WriteToStreamAsync(new RoomUserWhisperWriter
                    {
                        SenderId = roomUser.Id,
                        Message = effect.PlayerFurnitureItem!.MetaData,
                        EmotionId = 0,
                        Bubble = 0,
                        Unknown = 0
                    });
                }
                break;
            case FurnitureItemInteractionType.WiredEffectKickUser:
                break;
        }
    }
}