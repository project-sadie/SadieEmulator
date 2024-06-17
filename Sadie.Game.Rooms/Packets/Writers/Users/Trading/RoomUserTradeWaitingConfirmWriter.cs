using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeWaitingConfirm)]
public class RoomUserTradeWaitingConfirmWriter : AbstractPacketWriter
{
}