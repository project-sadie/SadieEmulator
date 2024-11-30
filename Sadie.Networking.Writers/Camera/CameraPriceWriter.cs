using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Camera;

[PacketId(ServerPacketId.CameraPrice)]
public class CameraPriceWriter : AbstractPacketWriter
{
    public required int CostCredits { get; init; }
    public required int CostPoints { get; init; }
    public required int PointsType { get; init; }
}