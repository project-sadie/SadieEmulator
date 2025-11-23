using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDimmerSave)]
public class RoomDimmerSaveEventHandler(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService,
    IMapper mapper) : INetworkPacketEventHandler
{
    public required int PresetId { get; init; }
    public required int BackgroundOnly { get; init; }
    public required string Color { get; init; }
    public required int Intensity { get; init; }
    public required bool Apply { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            return;
        }

        var dimmer = room
            .Room
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.Dimmer);

        if (dimmer == null)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var presets = dbContext
            .RoomDimmerPresets
            .Where(x => x.RoomId == room.Room.Id)
            .ToList();
        
        var preset = presets.FirstOrDefault(x => x.PresetId == PresetId);

        if (preset == null)
        {
            return;
        }

        preset.BackgroundOnly = BackgroundOnly == 2;
        preset.Color = Color;
        preset.Intensity = Intensity;

        room.Room.DimmerSettings.Enabled = Apply;

        var enabled = room.Room.DimmerSettings.Enabled ? 2 : 0;
        var bgOnly = preset.BackgroundOnly ? 2 : 0;
        var meta = $"{(enabled)},{preset.PresetId},{(bgOnly)},{preset.Color},{preset.Intensity}";
        
        await roomFurnitureItemHelperService.UpdateMetaDataForItemAsync(
            room, 
            dimmer,
            meta);

        dbContext.Entry(room.Room.DimmerSettings).Property(x => x.Enabled).IsModified = true;
        dbContext.Entry(preset).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        
        await room.UserRepository.BroadcastDataAsync(new RoomDimmerSettingsWriter
        {
            DimmerSettings = room.Room.DimmerSettings,
            DimmerPresets = mapper.Map<List<RoomDimmerPresetDto>>(presets)
        });
    }
}