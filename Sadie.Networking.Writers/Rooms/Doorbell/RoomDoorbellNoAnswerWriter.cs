using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

[PacketId(ServerPacketId.RoomDoorbellNoAnswer)]
public class RoomDoorbellNoAnswerWriter : AbstractPacketWriter
{
    public required string Username { get; init; }
}