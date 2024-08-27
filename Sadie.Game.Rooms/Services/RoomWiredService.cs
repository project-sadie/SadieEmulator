using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Services;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Packets.Writers.Users;

namespace Sadie.Game.Rooms.Services;

public class RoomWiredService : IRoomWiredService
{
    public async Task RunTriggerForRoomAsync(IRoomLogic room,
        PlayerFurnitureItemPlacementData trigger)
    {
        var effectsOnTrigger = GetEffectsForTrigger(trigger, room.FurnitureItems);

        foreach (var effect in effectsOnTrigger)
        {
            await RunEffectForRoomAsync(room, effect);
        }
    }
    
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
                foreach (var user in room.UserRepository.GetAll())
                {
                    var wiredMessage = effect.PlayerFurnitureItem!.MetaData;
                    
                    await user.Room.UserRepository.TryRemoveAsync(user.Id, true);
                    await user.Player.SendAlertAsync(wiredMessage);
                }
                break;
        }
    }
    
    public int GetWiredCode(string interactionType)
    {
        return interactionType switch
        {
            FurnitureItemInteractionType.WiredTriggerSaysSomething => (int) WiredTriggerCode.AvatarSaysSomething,
            FurnitureItemInteractionType.WiredTriggerEnterRoom => (int) WiredTriggerCode.AvatarEntersRoom,
            FurnitureItemInteractionType.WiredTriggerUserWalksOnFurniture => (int) WiredTriggerCode.AvatarWalksOnFurniture,
            FurnitureItemInteractionType.WiredTriggerUserWalksOffFurniture => (int) WiredTriggerCode.AvatarWalksOffFurniture,
            FurnitureItemInteractionType.WiredTriggerFurnitureStateChanged => (int) WiredTriggerCode.ToggleFurniture,
            FurnitureItemInteractionType.WiredEffectShowMessage => (int) WiredEffectCode.ShowMessage,
            FurnitureItemInteractionType.WiredEffectKickUser => (int) WiredEffectCode.KickUser,
            _ => throw new ArgumentException($"Couldn't match interaction type '{interactionType}' to a trigger layout.")
        };
    }

    public async Task SaveSettingsAsync(
        PlayerFurnitureItem playerItem,
        SadieContext dbContext,
        PlayerFurnitureItemWiredData wiredData)
    {
        if (playerItem.WiredData != null)
        {
            dbContext.Entry(playerItem.WiredData).State = EntityState.Deleted;
            
            foreach (var item in playerItem.WiredData.SelectedItems)
            {
                dbContext.Entry(item).State = EntityState.Deleted;
            }
            
            await dbContext.SaveChangesAsync();
        }

        playerItem.WiredData = wiredData;

        dbContext.Entry(playerItem).State = EntityState.Unchanged;
        await dbContext.SaveChangesAsync();
    }
}