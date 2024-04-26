using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomFurnitureItemRemoved)]
public class RoomFloorFurnitureItemRemovedWriter : NetworkPacketWriter
{
    public required string Id { get; init; }
    public required bool Expired { get; init; }
    public required long OwnerId { get; init; }
    public required int Delay { get; init; }
}