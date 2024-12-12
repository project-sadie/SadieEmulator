using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomHeightMap)]
public class RoomFloorHeightMapWriter : AbstractPacketWriter
{
    public required bool Scale { get; init; }
    public required int WallHeight { get; init; }
    public required string RelativeHeightmap { get; init; }
}