using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomUserTags)]
public class RoomUserTagsWriter : AbstractPacketWriter
{
    public required int UserId { get; init; }
    public required List<string> Tags { get; init; }
}