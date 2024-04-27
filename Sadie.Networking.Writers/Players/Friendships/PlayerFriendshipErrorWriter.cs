using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerFriendshipError)]
public class PlayerFriendshipErrorWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; init; }
    public required int Error { get; init; }
}