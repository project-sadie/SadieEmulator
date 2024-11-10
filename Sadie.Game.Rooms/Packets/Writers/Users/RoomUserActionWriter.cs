using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

[PacketId(ServerPacketId.RoomUserAction)]
public class RoomUserActionWriter : AbstractPacketWriter
{
    public required int UserId { get; set; }
    public required int Action { get; set; }
}