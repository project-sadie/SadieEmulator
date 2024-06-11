using System.Drawing;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.FloorPlanEditor;

[PacketId(ServerPacketId.FloorPlanEditorOccupiedTiles)]
public class FloorPlanEditorOccupiedTilesWriter : AbstractPacketWriter
{
    public required List<Point> Points { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(Points.Count);

        foreach (var point in Points)
        {
            writer.WriteInteger(point.X);
            writer.WriteInteger(point.Y);
        }
    }
}