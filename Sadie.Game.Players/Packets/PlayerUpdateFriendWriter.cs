using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerUpdateFriend)]
public class PlayerUpdateFriendWriter : AbstractPacketWriter
{
    public int Unknown1 { get; set; }
    public int Unknown2 { get; set; }
    public int Unknown3 { get; set; }
    public PlayerFriendship Friendship { get; set; }
    public bool IsOnline { get; set; }
    public bool CanFollow { get; set; }
    public int CategoryId { get; set; }
    public string RealName { get; set; }
    public string LastAccess { get; set; }
    public bool PersistedMessageUser { get; set; }
    public bool VipMember { get; set; }
    public bool PocketUser { get; set; }
    public int RelationshipType { get; set; }
}