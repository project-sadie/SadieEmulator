using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomWallFloorSettings)]
public class RoomWallFloorSettingsWriter : AbstractPacketWriter
{
    public required bool HideWalls { get; init; }
    public required int WallThickness { get; init; }
    public required int FloorThickness { get; init; }
}