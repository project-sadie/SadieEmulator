using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserHotelView)]
public class RoomUserHotelViewWriter : AbstractPacketWriter;