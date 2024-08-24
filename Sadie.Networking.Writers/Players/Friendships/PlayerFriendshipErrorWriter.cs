using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerFriendshipError)]
public class PlayerFriendshipErrorWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; init; }
    public required int Error { get; init; }
}