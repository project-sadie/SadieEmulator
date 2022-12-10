using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Rooms;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerFriendsListWriter : NetworkPacketWriter
{
    public PlayerFriendsListWriter(
        int pages, 
        int index, 
        List<PlayerFriendship> friends, 
        IPlayerRepository playerRepository, 
        IRoomRepository roomRepository)
    {
        WriteShort(ServerPacketId.PlayerFriendsList);
        WriteInteger(pages);
        WriteInteger(index);
        WriteInteger(friends.Count);

        foreach (var friend in friends)
        {
            var friendData = friend.TargetData;
            var isOnline = playerRepository.TryGetPlayerById(friendData.Id, out var onlineFriend) && onlineFriend != null;
            var inRoom = false;

            if (isOnline && onlineFriend != null)
            {
                var (roomFound, lastRoom) = roomRepository.TryGetRoomById(onlineFriend.Data.CurrentRoomId);

                if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(onlineFriend.Data.Id, out _))
                {
                    inRoom = true;
                }
            }

            WriteInteger(friendData.Id);
            WriteString(friendData.Username);
            WriteInteger(friendData.Gender == AvatarGender.Male ? 0 : 1);
            WriteBool(isOnline);
            WriteBool(inRoom);
            WriteString(friendData.FigureCode);
            WriteInteger(0); // unknown
            WriteString(friendData.Motto);
            WriteString(""); // unknown
            WriteString(""); // unknown
            WriteBool(false); // TODO: offline messaging
            WriteBool(false); // unknown
            WriteBool(false); // unknown
            WriteShort((short) friend.Type);
        }
    }
}