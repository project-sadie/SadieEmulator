using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted;
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
        var targetData = friendship.TargetPlayer;
        
        WriteShort(ServerPacketId.PlayerUpdateFriend);
        WriteInteger(unknown1);
        WriteInteger(unknown2);
        WriteInteger(unknown3);
        WriteInteger(targetData.Id);
        WriteString(targetData.Username);
        WriteInteger(targetData.AvatarData.Gender == AvatarGender.Male ? 0 : 1);
        WriteBool(isOnline);
        WriteBool(canFollow);
        WriteString(targetData.AvatarData.FigureCode);
        WriteInteger(categoryId);
        WriteString(targetData.AvatarData.Motto);
        WriteString(realName);
        WriteString(lastAccess);
        WriteBool(persistedMessageUser);
        WriteBool(vipMember);
        WriteBool(pocketUser);
        WriteShort((short) relationshipType);
    }
}