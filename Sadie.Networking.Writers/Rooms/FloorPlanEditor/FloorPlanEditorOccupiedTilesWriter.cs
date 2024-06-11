using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.FloorPlanEditor;

[PacketId(ServerPacketId.FloorPlanEditorOccupiedTiles)]
public class FloorPlanEditorOccupiedTilesWriter : AbstractPacketWriter
{
    public required Dictionary<int, int> Tiles { get; init; }
}