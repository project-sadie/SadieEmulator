using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomForwardEntry)]
public class RoomForwardEntryWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
}