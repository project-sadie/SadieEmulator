using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Relationships;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Players.Packets;

public class PlayerUpdateFriendWriter : NetworkPacketWriter
{
    public PlayerUpdateFriendWriter(
        int unknown1, 
        int unknown2, 
        int unknown3, 
        PlayerFriendship friendship, 
        bool isOnline, 
        bool inRoom, 
        int unknown4, 
        string unknown5, 
        string unknown6, 
        bool unknown7, 
        bool unknown8, 
        bool unknown9,
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
        WriteBool(inRoom);
        WriteString(targetData.FigureCode);
        WriteInteger(unknown4);
        WriteString(targetData.Motto);
        WriteString(unknown5);
        WriteString(unknown6);
        WriteBool(unknown7);
        WriteBool(unknown8);
        WriteBool(unknown9);
        WriteShort((short) relationshipType);
    }
}