using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.FloorPlanEditor;

[PacketId(ServerPacketId.FloorPlanEditorDoorCoords)]
public class FloorPlanEditorDoorCoordsWriter : AbstractPacketWriter
{
    public required int X { get; init; }
    public required int Y { get; init; }
    public required int Direction { get; init; }
}