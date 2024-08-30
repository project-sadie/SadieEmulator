using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserRespect)]
public class RoomUserRespectWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int TotalRespects { get; init; }
}