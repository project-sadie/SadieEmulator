using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Packets.Writers.Users;

namespace Sadie.Game.Rooms.Services;

public class RoomWiredService(IRoomFurnitureItemHelperService furnitureItemHelperService) : IRoomWiredService
{
    public IEnumerable<PlayerFurnitureItemPlacementData> GetTriggers(
        string interactionType,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems,
        string requiredMessage = "",
        List<int>? requiredSelectedIds = null)
    {
        return roomItems.Where(x =>
            x.WiredData != null &&
            x.PlayerFurnitureItem.FurnitureItem.InteractionType == interactionType &&
            (string.IsNullOrWhiteSpace(requiredMessage) || x.WiredData!.Message == requiredMessage) &&
            (requiredSelectedIds == null ||
             requiredSelectedIds.All(r => x.WiredData.SelectedItems.Select(i => i.Id)
                 .Contains(r))));
    }
    
    public async Task RunTriggerForRoomAsync(IRoomLogic room,
        PlayerFurnitureItemPlacementData trigger,
        IRoomUser userWhoTriggered)
    {
        CycleInteractionStateAsync(room, trigger);
        
        var effectsOnTrigger = GetEffectsForTrigger(trigger, room.FurnitureItems);

        foreach (var effect in effectsOnTrigger)
        {
            await RunEffectForRoomAsync(room, effect, userWhoTriggered);
        }
    }
    
    public IEnumerable<PlayerFurnitureItemPlacementData> GetEffectsForTrigger(
        PlayerFurnitureItemPlacementData trigger,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems)
    {
        var stack = roomItems
            .Where(x =>
                x.PositionX == trigger.PositionX &&
                x.PositionY == trigger.PositionY &&
                x.PositionZ > trigger.PositionZ)
            .OrderBy(x => x.PositionZ);
        
        foreach (var playerFurnitureItemPlacementData in stack)
        {
            if (!playerFurnitureItemPlacementData
                    .FurnitureItem
                    .InteractionType
                    .Contains("_act_"))
            {
                break;
            }
            
            yield return playerFurnitureItemPlacementData;
        }
    }
    
    private async Task RunEffectForRoomAsync(
        IRoomLogic room,
        PlayerFurnitureItemPlacementData effect,
        IRoomUser userWhoTriggered)
    {
        if (effect.WiredData == null)
        {
            return;
        }
        
        switch (effect.FurnitureItem.InteractionType)
        {
            case FurnitureItemInteractionType.WiredEffectShowMessage:
                await userWhoTriggered.NetworkObject.WriteToStreamAsync(new RoomUserWhisperWriter
                {
                    SenderId = userWhoTriggered.Id,
                    Message = effect.WiredData.Message,
                    EmotionId = 0,
                    Bubble = (int) ChatBubble.Alert,
                    Unknown = 0
                });
                break;
            case FurnitureItemInteractionType.WiredEffectKickUser:
                foreach (var user in room.UserRepository.GetAll())
                {
                    await user.Room.UserRepository.TryRemoveAsync(user.Id, true, true);
                    await user.Player.SendAlertAsync(effect.WiredData.Message);
                }
                break;
        }
        
        CycleInteractionStateAsync(room, effect);
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
        PlayerFurnitureItemPlacementData placementData,
        SadieContext dbContext,
        PlayerFurnitureItemWiredData wiredData)
    {
        var existingData = placementData.WiredData;
        
        if (existingData != null)
        {
            dbContext.Entry(existingData).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
        }

        placementData.WiredData = wiredData;

        dbContext.Entry(wiredData.PlacementData).State = EntityState.Unchanged;
        dbContext.Entry(wiredData).State = EntityState.Added;
        
        await dbContext.SaveChangesAsync();
    }

    private async Task CycleInteractionStateAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item)
    {
        await furnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "1");
        await Task.Delay(500);
        await furnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
    }
}