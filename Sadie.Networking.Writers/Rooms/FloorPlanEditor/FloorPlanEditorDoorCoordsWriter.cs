using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.FloorPlanEditor;

[PacketId(ServerPacketId.FloorPlanEditorDoorCoords)]
public class FloorPlanEditorDoorCoordsWriter : AbstractPacketWriter
{
    public required int X { get; init; }
    public required int Y { get; init; }
    public required int Direction { get; init; }
}