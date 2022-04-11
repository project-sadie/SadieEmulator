using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Rooms;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friends;

public class PlayerFriendsListWriter : NetworkPacketWriter
{
    public PlayerFriendsListWriter(int pages, int index, List<PlayerFriendship> friends, IPlayerRepository playerRepository, IRoomRepository roomRepository)
    {
        WriteShort(ServerPacketId.PlayerFriendsList);
        WriteInt(pages);
        WriteInt(index);
        WriteInt(friends.Count);

        foreach (var friend in friends)
        {
            var friendData = friend.TargetData;
            var isOnline = playerRepository.TryGetPlayerById(friendData.Id, out var onlineFriend) && onlineFriend != null;
            var inRoom = false;

            if (isOnline && onlineFriend != null)
            {
                var (roomFound, lastRoom) = roomRepository.TryGetRoomById(onlineFriend.LastRoomLoaded);

                if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(onlineFriend.Id, out _))
                {
                    inRoom = true;
                }
            }

            WriteInt(friendData.Id);
            WriteString(friendData.Username);
            WriteInt(friendData.Gender == AvatarGender.Male ? 0 : 1);
            WriteBoolean(isOnline);
            WriteBoolean(inRoom);
            WriteString(friendData.FigureCode);
            WriteInt(0); // unknown
            WriteString(friendData.Motto);
            WriteString(""); // unknown
            WriteString(""); // unknown
            WriteBoolean(false); // TODO: offline messaging
            WriteBoolean(false); // unknown
            WriteBoolean(false); // unknown
            WriteShort((short) friend.Type);
        }
    }
}