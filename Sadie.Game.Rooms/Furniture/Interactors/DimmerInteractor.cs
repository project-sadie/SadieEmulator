using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Game.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DimmerInteractor(SadieContext dbContext) : AbstractRoomFurnitureItemInteractor
{
    public override List<string> InteractionTypes => [FurnitureItemInteractionType.Dimmer];
    
    public override async Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
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

    public override async Task OnPickUpAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUser roomUser)
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
}