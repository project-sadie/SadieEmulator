using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDimmerToggle)]
public class RoomDimmerToggleEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!RoomContextResolver.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _) ||
            room.Room.DimmerSettings == null ||
            client.RoomUser == null ||
            !client.RoomUser.HasRights())
        {
            return;
        }

        var dimmer = room
            .Room.FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer);

        if (dimmer == null)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var preset = dbContext.RoomDimmerPresets
            .FirstOrDefault(x => x.RoomId == room.Room.Id && x.PresetId == room.Room.DimmerSettings.PresetId);

        if (preset == null)
        {
            return;
        }
        
        room.Room.DimmerSettings.Enabled = !room.Room.DimmerSettings.Enabled;

        var enabled = room.Room.DimmerSettings.Enabled ? 2 : 1;
        var bgOnly = preset.BackgroundOnly ? 2 : 0;
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(
            room, 
            dimmer, 
            $"{enabled},{preset.PresetId},{bgOnly},{preset.Color},{preset.Intensity}");
        
        dbContext.Entry(room.Room.DimmerSettings).Property(x => x.Enabled).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}