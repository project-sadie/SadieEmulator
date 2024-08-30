using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerMessengerInit)]
public class PlayerMessengerInitWriter : AbstractPacketWriter
{
    public required int MaxFriends { get; init; }
    public required int Unknown1 { get; init; }
    public required int MaxFriendsHc { get; init; }
    public required int Unknown2 { get; init; }
}