using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users.HandItems;

[PacketId(ServerPacketId.RoomUserReceivedHandItem)]
public class RoomUserReceivedHandItemWriter : AbstractPacketWriter
{
    public required long FromId { get; set; }
    public required int HandItemId { get; set; }
}