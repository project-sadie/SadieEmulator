using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomBannedUsers)]
public class RoomBannedUsersWriter : AbstractPacketWriter
{
    public required int RoomId { get; init; }
    public required Dictionary<long, string> BannedUsersMap { get; init; }
}