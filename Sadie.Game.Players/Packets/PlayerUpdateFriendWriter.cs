using Sadie.Game.Players.Friendships;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerUpdateFriendWriter : NetworkPacketWriter
{
    public PlayerUpdateFriendWriter(PlayerFriendship friendship, bool isOnline, bool inRoom)
    {
        var targetData = friendship.TargetData;
        
        WriteShort(ServerPacketId.PlayerUpdateFriend);
        WriteInteger(0); // unknown
        WriteInteger(1); // unknown
        WriteInteger(0); // unknown
        WriteInteger(targetData.Id);
        WriteString(targetData.Username);
        WriteInteger(targetData.Gender == AvatarGender.Male ? 0 : 1);
        WriteBool(isOnline);
        WriteBool(inRoom);
        WriteString(targetData.FigureCode);
        WriteInteger(0); // unknown
        WriteString(targetData.Motto);
        WriteString(""); // unknown
        WriteString(""); // unknown
        WriteBool(false); // TODO: offline messaging
        WriteBool(false); // unknown
        WriteBool(false); // unknown
        WriteShort((short) friendship.Type);
    }
}