using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users.HandItems;

[PacketId(ServerPacketId.RoomUserHandItem)]
public class RoomUserHandItemWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required int ItemId { get; init; }
}