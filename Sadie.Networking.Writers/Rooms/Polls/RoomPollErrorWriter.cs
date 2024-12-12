using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Polls;

[PacketId(ServerPacketId.RoomPollError)]
public class RoomPollErrorWriter : AbstractPacketWriter;