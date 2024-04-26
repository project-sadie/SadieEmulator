using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerMessengerInit)]
public class PlayerMessengerInitWriter : AbstractPacketWriter
{
    public required int MaxFriends { get; init; }
    public required int Unknown1 { get; init; }
    public required int MaxFriendsHc { get; init; }
    public required int Unknown2 { get; init; }
}