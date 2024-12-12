using Sadie.API.Game.Players.Friendships;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerFriendRequests)]
public class PlayerFriendRequestsWriter : AbstractPacketWriter
{
    public required int TotalRequests { get; init; }
    public required List<IPlayerFriendshipRequestData> Requests { get; init; }
}