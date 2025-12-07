using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserShout)]
public class RoomUserShoutWriter : RoomUserChatWriter;