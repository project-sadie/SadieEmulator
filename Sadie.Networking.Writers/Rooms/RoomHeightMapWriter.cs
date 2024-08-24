using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomHeightMap)]
public class RoomHeightMapWriter : AbstractPacketWriter
{
    public required bool Unknown1 { get; init; }
    public required int WallHeight { get; init; }
    public required string RelativeHeightmap { get; init; }
}