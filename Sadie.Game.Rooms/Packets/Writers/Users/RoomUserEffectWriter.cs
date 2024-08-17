using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Users;

[PacketId(ServerPacketId.RoomUserEffect)]
public class RoomUserEffectWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int EffectId { get; init; }
    public required int DelayMs { get; init; }
}