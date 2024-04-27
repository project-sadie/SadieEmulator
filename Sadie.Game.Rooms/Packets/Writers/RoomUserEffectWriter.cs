using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserEffectWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int EffectId { get; init; }
    public required int DelayMs { get; init; }
}