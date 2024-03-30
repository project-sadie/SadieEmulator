using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Relationships;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Players.Packets;

public class PlayerUpdateFriendWriter : NetworkPacketWriter
{
    public PlayerUpdateFriendWriter(
        int unknown1, 
        int unknown2, 
        int unknown3, 
        PlayerFriendship friendship, 
        bool isOnline, 
        bool canFollow, 
        int categoryId, 
        string realName, 
        string lastAccess, 
        bool persistedMessageUser, 
        bool vipMember, 
        bool pocketUser,
        PlayerRelationshipType relationshipType)
    {
        var targetData = friendship.TargetData;
        
        WriteShort(ServerPacketId.PlayerUpdateFriend);
        WriteInteger(unknown1);
        WriteInteger(unknown2);
        WriteInteger(unknown3);
        WriteInteger(targetData.Id);
        WriteString(targetData.Username);
        WriteInteger(targetData.Gender == AvatarGender.Male ? 0 : 1);
        WriteBool(isOnline);
        WriteBool(canFollow);
        WriteString(targetData.FigureCode);
        WriteInteger(categoryId);
        WriteString(targetData.Motto);
        WriteString(realName);
        WriteString(lastAccess);
        WriteBool(persistedMessageUser);
        WriteBool(vipMember);
        WriteBool(pocketUser);
        WriteShort((short) relationshipType);
    }
}