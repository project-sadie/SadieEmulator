using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Services;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Enums.Game.Rooms.Furniture;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Db;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Game.Rooms.Services;

public class RoomWiredService(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomFurnitureItemHelperService furnitureItemHelperService) : IRoomWiredService
{
    public IEnumerable<PlayerFurnitureItemPlacementDataDto> GetTriggers(
        string interactionType,
        IEnumerable<PlayerFurnitureItemPlacementDataDto> roomItems,
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
        PlayerFurnitureItemPlacementDataDto trigger,
        IRoomUser userWhoTriggered)
    {
        _ = CycleInteractionStateAsync(room, trigger);
        
        var effectsOnTrigger = GetEffectsForTrigger(trigger, room.Room.FurnitureItems);

        foreach (var effect in effectsOnTrigger)
        {
            await RunEffectForRoomAsync(room, effect, userWhoTriggered);
        }
    }
    
    public IEnumerable<PlayerFurnitureItemPlacementDataDto> GetEffectsForTrigger(
        PlayerFurnitureItemPlacementDataDto trigger,
        IEnumerable<PlayerFurnitureItemPlacementDataDto> roomItems)
    {
        var stack = roomItems
            .Where(x =>
                x.PositionX == trigger.PositionX &&
                x.PositionY == trigger.PositionY &&
                x.PositionZ > trigger.PositionZ)
            .OrderBy(x => x.PositionZ);
        
        foreach (var playerFurnitureItemPlacementData in stack)
        {
            var interactionType = playerFurnitureItemPlacementData
                .PlayerFurnitureItem
                .FurnitureItem
                .InteractionType;
            
            if (!string.IsNullOrEmpty(interactionType) && 
                !interactionType.Contains("_act_"))
            {
                break;
            }
            
            yield return playerFurnitureItemPlacementData;
        }
    }
    
    private async Task RunEffectForRoomAsync(
        IRoomLogic room,
        PlayerFurnitureItemPlacementDataDto effect,
        IRoomUser userWhoTriggered)
    {
        if (effect.WiredData == null)
        {
            return;
        }
        
        switch (effect.PlayerFurnitureItem.FurnitureItem.InteractionType)
        {
            case FurnitureItemInteractionType.WiredEffectShowMessage:
                await userWhoTriggered.NetworkObject.WriteToStreamAsync(new RoomUserWhisperWriter
                {
                    SenderId = userWhoTriggered.Player.Player.Id,
                    Message = effect.WiredData.Message,
                    EmotionId = 0,
                    ChatBubbleId = (int)ChatBubble.Alert,
                    MessageLength = effect.WiredData.Message.Length,
                    Urls = []
                });
                break;
            case FurnitureItemInteractionType.WiredEffectKickUser:
                foreach (var user in room.UserRepository.GetAll())
                {
                    await user.Room.UserRepository.TryRemoveAsync(user.Player.Player.Id, true, true);
                    await user.Player.SendAlertAsync(effect.WiredData.Message);
                }
                break;
        }
        
        _ = CycleInteractionStateAsync(room, effect);
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
        PlayerFurnitureItemPlacementDataDto placementData,
        PlayerFurnitureItemWiredDataDto wiredData)
    {
        var existingData = placementData.WiredData;
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
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

    private async Task CycleInteractionStateAsync(IRoomLogic room, PlayerFurnitureItemPlacementDataDto item)
    {
        await furnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "1");
        await Task.Delay(500);
        await furnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
    }
}