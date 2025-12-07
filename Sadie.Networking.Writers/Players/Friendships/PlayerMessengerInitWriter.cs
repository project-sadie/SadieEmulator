using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerMessengerInit)]
public class PlayerMessengerInitWriter : AbstractPacketWriter
{
    public required int UserFriendLimit { get; init; }
    public required int NormalFriendLimit { get; init; }
    public required int ExtendedFriendLimit { get; init; }
    public required Dictionary<int, string> FriendshipCategories { get; init; }
}