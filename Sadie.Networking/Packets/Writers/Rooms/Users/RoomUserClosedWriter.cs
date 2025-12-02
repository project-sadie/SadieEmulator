using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserClosed)]
public class RoomUserClosedWriter : AbstractPacketWriter;