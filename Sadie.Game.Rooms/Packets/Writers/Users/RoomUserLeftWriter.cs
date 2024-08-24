using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

[PacketId(ServerPacketId.RoomUserLeft)]
public class RoomUserLeftWriter : AbstractPacketWriter
{
    public required string UserId { get; init; }
}