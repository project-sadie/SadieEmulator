using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomDimmerSave)]
public class RoomDimmerSaveEventHandler(
    RoomRepository roomRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public required int Id { get; init; }
    public required int BackgroundOnly { get; init; }
    public required string Color { get; init; }
    public required int Intensity { get; init; }
    public required bool Apply { get; init; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            return;
        }

        var presets = dbContext.RoomDimmerPresets.Where(x => x.RoomId == room.Id).ToList();
        var preset = presets.FirstOrDefault(x => x.PresetId == Id);

        if (preset == null)
        {
            return;
        }

        preset.BackgroundOnly = BackgroundOnly == 2;
        preset.Color = Color;
        preset.Intensity = Intensity;

        room.DimmerSettings.Enabled = Apply;

        dbContext.Entry(room.DimmerSettings).Property(x => x.Enabled).IsModified = true;
        dbContext.Entry(preset).State = EntityState.Modified;
        
        await dbContext.SaveChangesAsync();
        
        await room.UserRepository.BroadcastDataAsync(new RoomDimmerSettingsWriter
        {
            DimmerSettings = room.DimmerSettings,
            DimmerPresets = presets
        });
    }
}