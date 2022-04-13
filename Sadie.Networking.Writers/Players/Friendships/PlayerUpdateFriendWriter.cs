using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Rooms;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerUpdateFriendWriter : NetworkPacketWriter
{
    public PlayerUpdateFriendWriter(PlayerFriendship friendship, IPlayerRepository playerRepository, IRoomRepository roomRepository)
    {
        var targetData = friendship.TargetData;
        var isOnline = playerRepository.TryGetPlayerById(targetData.Id, out var onlineFriend) && onlineFriend != null;
        var inRoom = false;

        if (isOnline && onlineFriend != null)
        {
            var onlineData = onlineFriend.Data;
            var (roomFound, lastRoom) = roomRepository.TryGetRoomById(onlineData.LastRoomLoaded);

            if (roomFound && lastRoom != null && lastRoom.UserRepository.TryGet(onlineData.Id, out _))
            {
                inRoom = true;
            }
        }
        
        WriteShort(ServerPacketId.PlayerUpdateFriend);
        WriteInt(0); // unknown
        WriteInt(1); // unknown
        WriteInt(0); // unknown
        WriteInt(targetData.Id);
        WriteString(targetData.Username);
        WriteInt(targetData.Gender == AvatarGender.Male ? 0 : 1);
        WriteBoolean(isOnline);
        WriteBoolean(inRoom);
        WriteString(targetData.FigureCode);
        WriteInt(0); // unknown
        WriteString(targetData.Motto);
        WriteString(""); // unknown
        WriteString(""); // unknown
        WriteBoolean(false); // TODO: offline messaging
        WriteBoolean(false); // unknown
        WriteBoolean(false); // unknown
        WriteShort((short) friendship.Type);
    }
}