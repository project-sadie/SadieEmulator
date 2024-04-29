using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

[PacketId(ServerPacketId.RoomDoorbellAccept)]
public class RoomDoorbellAcceptWriter : AbstractPacketWriter
{
    public required string Username { get; init; }
}