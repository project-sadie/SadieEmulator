using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerFriendsList)]
public class PlayerFriendsListWriter : AbstractPacketWriter
{
    [PacketData] public required int Pages { get; init; }
    [PacketData] public required int Index { get; init; }
    public required int PlayerId { get; init; }
    public required ICollection<PlayerFriendship> Friends { get; init; }
    public required PlayerRepository PlayerRepository { get; init; }
    public required ICollection<PlayerRelationship> Relationships { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(Pages);
        writer.WriteInteger(Index);
        writer.WriteInteger(Friends.Count);

        foreach (var friend in Friends)
        {
            var friendData = friend.OriginPlayerId == PlayerId ? 
                friend.TargetPlayer : 
                friend.OriginPlayer;
            
            var onlineFriend = PlayerRepository.GetPlayerLogicById(friendData.Id);
            var isOnline = onlineFriend != null;
            var inRoom = isOnline && onlineFriend != null && onlineFriend.CurrentRoomId != 0;
            var relationshipType = Relationships.FirstOrDefault(x => x.TargetPlayerId == friendData.Id)?.TypeId ?? PlayerRelationshipType.None;

            writer.WriteInteger(friendData.Id);
            writer.WriteString(friendData.Username);
            writer.WriteInteger(friendData.AvatarData.Gender == AvatarGender.Male ? 0 : 1);
            writer.WriteBool(isOnline);
            writer.WriteBool(inRoom);
            writer.WriteString(friendData.AvatarData.FigureCode);
            writer.WriteInteger(0); // unknown
            writer.WriteString(friendData.AvatarData.Motto);
            writer.WriteString(friendData.Username); // real name
            writer.WriteString(""); // last access?
            writer.WriteBool(false); // TODO: offline messaging
            writer.WriteBool(false); // unknown
            writer.WriteBool(false); // unknown
            writer.WriteShort((short) relationshipType);
        }
    }
}
