using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeStatus)]
public class RoomUserTradeStatusWriter : AbstractPacketWriter
{
    public required int UserId { get; set; }
    public required int Status { get; set; }
}