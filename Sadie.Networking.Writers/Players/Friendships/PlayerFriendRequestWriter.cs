using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Friendships;

[PacketId(ServerPacketId.PlayerFriendRequest)]
public class PlayerFriendRequestWriter : AbstractPacketWriter
{
    public required long Id { get; set; }
    public required string Username { get; set; }
    public required string FigureCode { get; set; }
}