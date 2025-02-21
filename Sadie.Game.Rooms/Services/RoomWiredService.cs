using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Game.Rooms.Wired.Effects;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Wired.Effects;

namespace Sadie.Game.Rooms.Services;

public class RoomWiredService(IRoomFurnitureItemHelperService furnitureItemHelperService) : IRoomWiredService
{
    public IEnumerable<PlayerFurnitureItemPlacementData> GetTriggers(
        string interactionType,
        IEnumerable<PlayerFurnitureItemPlacementData> roomItems,
        string requiredMessageIfExists = "",
        List<int>? requiredSelectedIds = null)
    {
        return roomItems.Where(x =>
            x.WiredData != null &&
            x.PlayerFurnitureItem.FurnitureItem.InteractionType == interactionType &&
            (string.IsNullOrWhiteSpace(x.WiredData.Message) || x.WiredData.Message == requiredMessageIfExists) &&
            (requiredSelectedIds == null ||
             requiredSelectedIds.All(r => x.WiredData.PlayerFurnitureItemWiredItems.Select(i => i.PlayerFurnitureItemPlacementDataId)
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
            RunEffectForRoomAsync(room, effect, userWhoTriggered);
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
    
    private readonly Dictionary<string, IWiredEffectRunner> _effectRunnerMap = new()
    {
        { FurnitureItemInteractionType.WiredEffectShowMessage, new ShowMessageEffectRunner() },
        { FurnitureItemInteractionType.WiredEffectKickUser, new KickUserEffectRunner() },
        { FurnitureItemInteractionType.WiredEffectTeleportToFurniture, new TeleportToFurnitureEffectRunner() },
    };

    private async Task RunEffectForRoomAsync(
        IRoomLogic room,
        PlayerFurnitureItemPlacementData effect,
        IRoomUser userWhoTriggered)
    {
        if (effect.WiredData == null || 
            !_effectRunnerMap.TryGetValue(effect.FurnitureItem.InteractionType, out var runner))
        {
            return;
        }
        
        if (effect.WiredData.Delay > 0)
        {
            await Task.Delay(effect.WiredData.Delay * 500);
        }

        await runner.ExecuteAsync(room, userWhoTriggered, effect);
        await CycleInteractionStateAsync(room, effect);
    }
    
    public int GetTriggerCodeFromInteractionType(string interactionType)
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
            FurnitureItemInteractionType.WiredEffectTeleportToFurniture => (int) WiredEffectCode.TeleportToFurniture,
            _ => throw new ArgumentException($"Couldn't match interaction type '{interactionType}' to a trigger layout.")
        };
    }

    public int GetSelectionCodeFromInteractionType(string interactionType)
    {
        return interactionType switch
        {
            FurnitureItemInteractionType.WiredTriggerUserWalksOnFurniture => 0,
            _ => 1
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

        dbContext.Entry(wiredData).State = EntityState.Added;
        dbContext.Entry(wiredData.PlacementData).State = EntityState.Unchanged;
        
        await dbContext.SaveChangesAsync();
        
        placementData.WiredData = wiredData;
    }

    private async Task CycleInteractionStateAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item)
    {
        await furnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "1");
        await Task.Delay(500);
        await furnitureItemHelperService.UpdateMetaDataForItemAsync(room, item, "0");
    }
}