using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserTags)]
public class RoomUserTagsWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required List<string> Tags { get; init; }
}