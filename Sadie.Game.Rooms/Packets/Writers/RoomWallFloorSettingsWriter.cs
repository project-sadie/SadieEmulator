using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomWallFloorSettings)]
public class RoomWallFloorSettingsWriter : AbstractPacketWriter
{
    public required bool HideWalls { get; init; }
    public required int WallThickness { get; init; }
    public required int FloorThickness { get; init; }
}