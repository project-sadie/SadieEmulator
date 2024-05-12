using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerFriendRequest)]
public class PlayerFriendRequestWriter : AbstractPacketWriter
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    public required string FigureCode { get; set; }
}