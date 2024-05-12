using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomPaint)]
public class RoomPaintWriter : AbstractPacketWriter
{
    public required string Type { get; init; }
    public required string Value { get; init; }
}