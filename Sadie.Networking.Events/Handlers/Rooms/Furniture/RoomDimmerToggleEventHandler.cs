using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomDimmerToggle)]
public class RoomDimmerToggleEventHandler(
    SadieContext dbContext,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            return;
        }

        var dimmer = room
            .FurnitureItems
            .FirstOrDefault(x => x.FurnitureItem.InteractionType == "dimmer");

        if (dimmer == null)
        {
            return;
        }

        var preset = dbContext.RoomDimmerPresets
            .FirstOrDefault(x => x.RoomId == room.Id && x.PresetId == room.DimmerSettings.PresetId);

        if (preset == null)
        {
            return;
        }
        
        room.DimmerSettings.Enabled = !room.DimmerSettings.Enabled;
        
        dimmer.PlayerFurnitureItem.MetaData = $"{(room.DimmerSettings.Enabled ? 2 : 1)},{preset.PresetId},{(preset.BackgroundOnly ? 2 : 0)},{preset.Color},{preset.Intensity}";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, dimmer);
        
        dbContext.Entry(room.DimmerSettings).Property(x => x.Enabled).IsModified = true;
        await dbContext.SaveChangesAsync();
    }
}