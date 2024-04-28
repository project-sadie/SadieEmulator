using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

[PacketId(ServerPacketId.RoomDoorbellNoAnswer)]
public class RoomDoorbellNoAnswerWriter : AbstractPacketWriter
{
    public required string Username { get; init; }
}