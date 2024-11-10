using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomPaint)]
public class RoomPaintWriter : AbstractPacketWriter
{
    public required string Type { get; init; }
    public required string Value { get; init; }
}