using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomUserHandItem)]
public class RoomUserHandItemWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int ItemId { get; init; }
}