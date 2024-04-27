using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerFriendRequest)]
public class PlayerFriendRequestWriter : AbstractPacketWriter
{
    public required int Id { get; set; }
    public required string Username { get; set; }
    public required string FigureCode { get; set; }
}