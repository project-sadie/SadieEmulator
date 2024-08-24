using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomForwardEntry)]
public class RoomForwardEntryWriter : AbstractPacketWriter
{
    public required long RoomId { get; init; }
}