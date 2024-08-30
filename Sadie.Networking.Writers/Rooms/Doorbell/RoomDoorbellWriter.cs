using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

[PacketId(ServerPacketId.RoomDoorbell)]
public class RoomDoorbellWriter : AbstractPacketWriter
{
    public required string Username { get; init; }
}