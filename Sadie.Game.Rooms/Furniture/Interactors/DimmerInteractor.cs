using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DimmerInteractor(SadieContext dbContext) : IRoomFurnitureItemInteractor
{
    public string InteractionType => "dimmer";
    
    public async Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
    }

    public async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        if (room.DimmerSettings == null)
        {
            var presetOne = new RoomDimmerPreset
            {
                RoomId = room.Id,
                PresetId = 1,
                BackgroundOnly = false,
                Color = "",
                Intensity = 255
            };

            var presetTwo = new RoomDimmerPreset
            {
                RoomId = room.Id,
                PresetId = 2,
                BackgroundOnly = false,
                Color = "",
                Intensity = 255
            };

            var presetThree = new RoomDimmerPreset
            {
                RoomId = room.Id,
                PresetId = 3,
                BackgroundOnly = false,
                Color = "",
                Intensity = 255
            };
            
            room.DimmerSettings = new RoomDimmerSettings
            {
                RoomId = room.Id,
                Enabled = false,
                PresetId = 1
            };

            dbContext.RoomDimmerPresets.Add(presetOne);
            dbContext.RoomDimmerPresets.Add(presetTwo);
            dbContext.RoomDimmerPresets.Add(presetThree);
            dbContext.RoomDimmerSettings.Add(room.DimmerSettings);

            await dbContext.SaveChangesAsync();
        }
    }

    public async Task OnPickUpAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        await dbContext
            .RoomDimmerPresets
            .Where(x => x.RoomId == room.Id)
            .ExecuteDeleteAsync();
     
        if (room.DimmerSettings != null)
        {
            room.DimmerSettings = null;
            
            dbContext.Entry(room.DimmerSettings).State = EntityState.Deleted;
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task OnMoveAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
    }

    public Task OnStepOnAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
    public Task OnStepOffAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit? roomUnit) => Task.CompletedTask;
}