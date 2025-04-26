using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.FloorPlanEditor;

namespace Sadie.Networking.Events.Handlers.Rooms.FloorPlanEditor;

[PacketId(EventHandlerId.FloorPlanEditorDoorCoords)]
public class FloorPlanEditorDoorCoordsEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _))
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