using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomData)]
public class RoomDataWriter : AbstractPacketWriter
{
    public required string LayoutName { get; init; }
    public required int RoomId { get; init; }
}