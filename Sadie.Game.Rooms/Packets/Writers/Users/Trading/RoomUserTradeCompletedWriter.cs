using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeCompleted)]
public class RoomUserTradeCompletedWriter : AbstractPacketWriter
{
}