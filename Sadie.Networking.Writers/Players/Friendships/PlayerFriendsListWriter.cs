using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerFriendsListWriter : NetworkPacketWriter
{
    public PlayerFriendsListWriter(
        int pages, 
        int index, 
        ICollection<PlayerFriendship> friends, 
        PlayerRepository playerRepository, 
        ICollection<PlayerRelationship> relationships)
    {
        WriteShort(ServerPacketId.PlayerFriendsList);
        WriteInteger(pages);
        WriteInteger(index);
        WriteInteger(friends.Count);

        foreach (var friend in friends)
        {
            var friendData = friend.TargetPlayer;
            var onlineFriend = playerRepository.GetPlayerLogicById(friendData.Id);
            var isOnline = onlineFriend != null;
            var inRoom = isOnline && onlineFriend != null && onlineFriend.CurrentRoomId != 0;
            var relationshipType = relationships.FirstOrDefault(x => x.TargetPlayerId == friendData.Id)?.TypeId ?? PlayerRelationshipType.None;

            WriteInteger(friendData.Id);
            WriteString(friendData.Username);
            WriteInteger(friendData.AvatarData.Gender == AvatarGender.Male ? 0 : 1);
            WriteBool(isOnline);
            WriteBool(inRoom);
            WriteString(friendData.AvatarData.FigureCode);
            WriteInteger(0); // unknown
            WriteString(friendData.AvatarData.Motto);
            WriteString(friendData.Username); // real name
            WriteString(""); // last access?
            WriteBool(false); // TODO: offline messaging
            WriteBool(false); // unknown
            WriteBool(false); // unknown
            WriteShort((short) relationshipType);
        }
    }
}