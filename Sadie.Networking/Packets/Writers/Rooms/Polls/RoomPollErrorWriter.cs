using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Polls;

[PacketId(ServerPacketId.RoomPollError)]
public class RoomPollErrorWriter : AbstractPacketWriter;