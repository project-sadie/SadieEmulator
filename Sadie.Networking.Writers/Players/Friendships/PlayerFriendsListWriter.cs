using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Relationships;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerFriendsListWriter : NetworkPacketWriter
{
    public PlayerFriendsListWriter(
        int pages, 
        int index, 
        List<PlayerFriendship> friends, 
        IPlayerRepository playerRepository, 
        List<PlayerRelationship> relationships)
    {
        WriteShort(ServerPacketId.PlayerFriendsList);
        WriteInteger(pages);
        WriteInteger(index);
        WriteInteger(friends.Count);

        foreach (var friend in friends)
        {
            var friendData = friend.TargetData;
            var isOnline = playerRepository.TryGetPlayerById(friendData.Id, out var onlineFriend) && onlineFriend != null;
            var inRoom = isOnline && onlineFriend != null && onlineFriend.Data.CurrentRoomId != 0;
            var relationshipType = relationships.FirstOrDefault(x => x.TargetPlayerId == friendData.Id)?.Type ?? PlayerRelationshipType.None;

            WriteInteger(friendData.Id);
            WriteString(friendData.Username);
            WriteInteger(friendData.Gender == AvatarGender.Male ? 0 : 1);
            WriteBool(isOnline);
            WriteBool(inRoom);
            WriteString(friendData.FigureCode);
            WriteInteger(0); // unknown
            WriteString(friendData.Motto);
            WriteString(friendData.Username); // real name
            WriteString(""); // last access?
            WriteBool(false); // TODO: offline messaging
            WriteBool(false); // unknown
            WriteBool(false); // unknown
            WriteShort((short) relationshipType);
        }
    }
}