using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Networking.Writers.Rooms.FloorPlanEditor;
using Sadie.Shared.Attributes;

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