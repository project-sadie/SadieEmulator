using System.Drawing;
using Sadie.API;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.FloorPlanEditor;

[PacketId(ServerPacketId.FloorPlanEditorOccupiedTiles)]
public class FloorPlanEditorOccupiedTilesWriter : AbstractPacketWriter
{
    public required List<Point> Points { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Points.Count);

        foreach (var point in Points)
        {
            writer.WriteInteger(point.X);
            writer.WriteInteger(point.Y);
        }
    }
}