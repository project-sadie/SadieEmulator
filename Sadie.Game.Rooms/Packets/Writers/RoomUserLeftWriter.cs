using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomUserLeft)]
public class RoomUserLeftWriter : AbstractPacketWriter
{
    public required string UserId { get; init; }
}