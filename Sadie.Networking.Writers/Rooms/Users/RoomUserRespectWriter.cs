using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserRespect)]
public class RoomUserRespectWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int TotalRespects { get; init; }
}