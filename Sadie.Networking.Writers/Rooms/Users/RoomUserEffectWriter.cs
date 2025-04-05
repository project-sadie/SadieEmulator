using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserEffect)]
public class RoomUserEffectWriter : AbstractPacketWriter
{
    public required long UserId { get; init; }
    public required int EffectId { get; init; }
    public required int DelayMs { get; init; }
}