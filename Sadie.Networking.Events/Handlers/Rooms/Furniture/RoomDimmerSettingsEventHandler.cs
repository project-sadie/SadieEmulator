using Sadie.Database;
using Sadie.Enums;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDimmerSettings)]
public class RoomDimmerSettingsEventHandler(
    SadieContext dbContext, 
    RoomRepository roomRepository) : INetworkPacketEventHandler
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
        
        await client.WriteToStreamAsync(new RoomDimmerSettingsWriter
        {
            DimmerSettings = room.DimmerSettings,
            DimmerPresets = dbContext.RoomDimmerPresets.Where(x => x.RoomId == room.Id).ToList()
        });
    }
}