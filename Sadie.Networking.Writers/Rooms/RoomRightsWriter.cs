using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomRights)]
public class RoomRightsWriter : AbstractPacketWriter
{
    public required int ControllerLevel { get; init; }
}