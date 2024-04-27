using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserDance)]
public class RoomUserDanceWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int DanceId { get; init; }
}