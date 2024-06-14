using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.FloorPlanEditor;

namespace Sadie.Networking.Events.Handlers.Rooms.FloorPlanEditor;

[PacketId(EventHandlerIds.FloorPlanEditorDoorCoords)]
public class FloorPlanEditorDoorCoordsEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        await client.WriteToStreamAsync(new FloorPlanEditorDoorCoordsWriter
        {
            X = room.Layout.DoorX,
            Y = room.Layout.DoorY,
            Direction = (int) room.Layout.DoorDirection
        });
    }
}