using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserEffect)]
public class RoomUserEffectWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int EffectId { get; init; }
    public required int DelayMs { get; init; }
}