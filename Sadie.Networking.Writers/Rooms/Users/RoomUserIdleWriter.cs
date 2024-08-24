using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserIdle)]
public class RoomUserIdleWriter : AbstractPacketWriter
{
    public required int UserId { get; set; }
    public required bool IsIdle { get; set; }
}