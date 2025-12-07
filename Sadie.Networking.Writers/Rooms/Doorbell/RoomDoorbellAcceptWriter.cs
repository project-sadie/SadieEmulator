using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Doorbell;

[PacketId(ServerPacketId.RoomDoorbellAccept)]
public class RoomDoorbellAcceptWriter : AbstractPacketWriter
{
    public required string Username { get; init; }
}