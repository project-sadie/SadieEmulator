using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers.Users.HandItems;

[PacketId(ServerPacketId.RoomUserReceivedHandItem)]
public class RoomUserReceivedHandItemWriter : AbstractPacketWriter
{
    public required long FromId { get; set; }
    public required int HandItemId { get; set; }
}