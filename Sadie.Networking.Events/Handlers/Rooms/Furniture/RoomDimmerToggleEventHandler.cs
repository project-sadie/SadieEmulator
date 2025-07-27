using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.Db;
using Sadie.Enums.Game.Furniture;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDimmerToggle)]
public class RoomDimmerToggleEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _) ||
            room.DimmerSettings == null ||
            client.RoomUser == null ||
            !client.RoomUser.HasRights())
        {
            return;
        }

        var dimmer = room
            .FurnitureItems
            .FirstOrDefault(x => x.FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer);

        if (dimmer == null)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var preset = dbContext.RoomDimmerPresets
            .FirstOrDefault(x => x.RoomId == room.Id && x.PresetId == room.DimmerSettings.PresetId);

        if (preset == null)
        {
            return;
        }
        
        room.DimmerSettings.Enabled = !room.DimmerSettings.Enabled;

        var enabled = room.DimmerSettings.Enabled ? 2 : 1;
        var bgOnly = preset.BackgroundOnly ? 2 : 0;
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(
            room, 
            dimmer, 
            $"{enabled},{preset.PresetId},{bgOnly},{preset.Color},{preset.Intensity}");
        
        dbContext.Entry(room.DimmerSettings).Property(x => x.Enabled).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}