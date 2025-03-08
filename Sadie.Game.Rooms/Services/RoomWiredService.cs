using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Game.Rooms.Wired.Effects.Actions;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Serilog;

namespace Sadie.Game.Rooms.Services;

public class RoomWiredService(
    IRoomFurnitureItemHelperService furnitureItemHelperService,
    IEnumerable<IWiredEffectAction> effectActions) : IRoomWiredService
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
    
    private async Task RunEffectForRoomAsync(
        IRoomLogic room,
        PlayerFurnitureItemPlacementData effect,
        IRoomUser userWhoTriggered)
    {
        if (effect.WiredData == null)
        {
            return;
        }

        var effectRunner = effectActions
            .FirstOrDefault(x => x.InteractionType == effect.FurnitureItem.InteractionType);

        if (effectRunner == null)
        {
            Log.Warning($"Failed to find effect runner for interaction type '{effect.FurnitureItem.InteractionType}'");
            return;
        }
        
        if (effect.WiredData.Delay > 0)
        {
            await Task.Delay(effect.WiredData.Delay * 500);
        }

        await effectRunner.ExecuteAsync(room, userWhoTriggered, effect);
        await CycleInteractionStateAsync(room, effect);
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

        foreach (var item in wiredData.PlayerFurnitureItemWiredItems)
        {
            dbContext.Entry(item).State = EntityState.Added;
        }
        
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