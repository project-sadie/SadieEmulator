using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeStatus)]
public class RoomUserTradeStatusWriter : AbstractPacketWriter
{
    public required int UserId { get; set; }
    public required int Status { get; set; }
}