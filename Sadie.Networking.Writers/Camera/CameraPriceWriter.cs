using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Camera;

[PacketId(ServerPacketId.CameraPrice)]
public class CameraPriceWriter : AbstractPacketWriter
{
    public required int CostCredits { get; init; }
    public required int CostPoints { get; init; }
    public required int PointsType { get; init; }
}