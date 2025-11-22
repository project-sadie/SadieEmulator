using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Db;
using Sadie.Networking.Writers.Rooms.Furniture;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDimmerSettings)]
public class RoomDimmerSettingsEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
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

        if (room.DimmerSettings == null)
        {
            throw new Exception("DIMMER_SETTINGS_NULL_WHEN_DIMMER_IN_ROOM");
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        await client.WriteToStreamAsync(new RoomDimmerSettingsWriter
        {
            DimmerSettings = room.DimmerSettings,
            DimmerPresets = dbContext.RoomDimmerPresets.Where(x => x.RoomId == room.Id).ToList()
        });
    }
}