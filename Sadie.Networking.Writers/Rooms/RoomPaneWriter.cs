using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomPane)]
public class RoomPaneWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required bool Owner { get; init; }
}