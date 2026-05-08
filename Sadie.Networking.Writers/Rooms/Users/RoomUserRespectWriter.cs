using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserRespect)]
public class RoomUserRespectWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int TotalRespects { get; init; }
}