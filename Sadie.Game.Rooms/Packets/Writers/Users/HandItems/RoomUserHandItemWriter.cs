using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Users.HandItems;

[PacketId(ServerPacketId.RoomUserHandItem)]
public class RoomUserHandItemWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int ItemId { get; init; }
}