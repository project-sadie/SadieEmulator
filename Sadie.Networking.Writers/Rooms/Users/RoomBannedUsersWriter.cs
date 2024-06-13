using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomBannedUsers)]
public class RoomBannedUsersWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required Dictionary<int, string> BannedUsersMap { get; init; }
}