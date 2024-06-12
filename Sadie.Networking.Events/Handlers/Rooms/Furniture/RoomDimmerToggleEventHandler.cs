using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomDimmerToggle)]
public class RoomDimmerToggleEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
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

        var dimmer = room
            .FurnitureItems
            .FirstOrDefault(x => x.FurnitureItem.InteractionType == "dimmer");

        if (dimmer == null)
        {
            return;
        }

        dimmer.MetaData = "2,1,2,#FF00FF,255";
        await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, dimmer);
    }
}