using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomRights)]
public class RoomRightsWriter : AbstractPacketWriter
{
    public required int ControllerLevel { get; init; }
}