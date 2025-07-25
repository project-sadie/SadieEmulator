using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Networking;
using Sadie.Db.Models.Players;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Writers;
using Sadie.Shared.Attributes;
using PlayerRelationshipType = Sadie.Enums.Game.Players.PlayerRelationshipType;

namespace Sadie.Game.Players.Packets.Writers;

[PacketId(ServerPacketId.PlayerFriendsList)]
public class PlayerFriendsListWriter : AbstractPacketWriter
{
    public required int Pages { get; init; }
    public required int Index { get; init; }
    public required long PlayerId { get; init; }
    public required ICollection<PlayerFriendship> Friends { get; init; }
    public required IPlayerRepository PlayerRepository { get; init; }
    public required ICollection<PlayerRelationship> Relationships { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
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
            var inRoom = isOnline && onlineFriend != null && onlineFriend.State.CurrentRoomId != 0;
            
            var relationshipType = Relationships
               .FirstOrDefault(x => x.TargetPlayerId == friendData.Id)
               ?.TypeId ?? (int) PlayerRelationshipType.None;

            writer.WriteLong(friendData.Id);
            writer.WriteString(friendData.Username);
            writer.WriteInteger(friendData.AvatarData.Gender == AvatarGender.Male ? 0 : 1);
            writer.WriteBool(isOnline);
            writer.WriteBool(inRoom);
            writer.WriteString(friendData.AvatarData.FigureCode);
            writer.WriteInteger(0); // category ID
            writer.WriteString(friendData.AvatarData.Motto ?? string.Empty);
            writer.WriteString(friendData.Username); // real name
            writer.WriteString(""); // last access?
            writer.WriteBool(false);
            writer.WriteBool(false); // VIP
            writer.WriteBool(false); // pocket
            writer.WriteShort((short) relationshipType);
        }
    }
}
