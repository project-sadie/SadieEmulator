using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.Trading;

[PacketId(ServerPacketId.RoomUserTradeCloseWindow)]
public class RoomUserTradeCloseWindowWriter : AbstractPacketWriter;