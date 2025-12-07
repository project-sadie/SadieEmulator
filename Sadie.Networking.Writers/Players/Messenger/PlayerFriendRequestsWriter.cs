using Sadie.API.Interfaces.Game.Players.Friendships;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerFriendRequests)]
public class PlayerFriendRequestsWriter : AbstractPacketWriter
{
    public required int TotalRequests { get; init; }
    public required List<IPlayerFriendshipRequestData> Requests { get; init; }
}