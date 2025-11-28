using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDimmerSettings)]
public class RoomDimmerSettingsEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IMapper mapper) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        var dimmer = room
            .Room.FurnitureItems
            .FirstOrDefault(x => x
                .PlayerFurnitureItem
                .FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer);

        if (dimmer == null)
        {
            return;
        }

        if (room.Room.DimmerSettings == null)
        {
            throw new Exception("DIMMER_SETTINGS_NULL_WHEN_DIMMER_IN_ROOM");
        }
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var dimmerPresets = dbContext
            .RoomDimmerPresets
            .Where(x => x.RoomId == room.Room.Id)
            .ToList();
        
        await client.WriteToStreamAsync(new RoomDimmerSettingsWriter
        {
            DimmerSettings = room.Room.DimmerSettings,
            DimmerPresets = mapper.Map<List<RoomDimmerPresetDto>>(dimmerPresets)
        });
    }
}