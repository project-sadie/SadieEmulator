using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeCompleted)]
public class RoomUserTradeCompletedWriter : AbstractPacketWriter;