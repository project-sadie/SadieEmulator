using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserChat)]
public class RoomUserChatWriter : RoomUserWhisperWriter;