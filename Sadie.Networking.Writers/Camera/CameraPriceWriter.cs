using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Camera;

[PacketId(ServerPacketId.CameraPrice)]
public class CameraPriceWriter : AbstractPacketWriter
{
    public required int CostCredits { get; init; }
    public required int CostPoints { get; init; }
    public required int PointsType { get; init; }
}