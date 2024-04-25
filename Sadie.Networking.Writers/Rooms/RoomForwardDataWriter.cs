using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomForwardDataWriter : NetworkPacketWriter
{
    public RoomForwardDataWriter(RoomLogic room, bool roomForward, bool enterRoom, bool isOwner)
    {
        var settings = room.Settings;
        var chatSettings = room.ChatSettings;
        
        WriteShort(ServerPacketId.RoomForwardData);
        WriteBool(enterRoom);
        WriteLong(room.Id);
        WriteString(room.Name);
        WriteLong(room.OwnerId);
        WriteString(room.Owner.Username);
        WriteInteger((int) room.Settings.AccessType);
        WriteInteger(room.UserRepository.Count);
        WriteInteger(room.MaxUsersAllowed);
        WriteString(room.Description);
        WriteInteger(settings.TradeOption);
        WriteInteger(2); // unknown
        WriteInteger(room.PlayerLikes.Count);
        WriteInteger(0); // category
        WriteInteger(room.Tags.Count);

        foreach (var tag in room.Tags)
        {
            WriteString(tag.Name);
        }
        
        WriteInteger(0 | 8); // TODO: base
        WriteBool(roomForward);
        WriteBool(false); // TODO: staff picked?
        WriteBool(false); // TODO: is group member?
        WriteBool(room.IsMuted);
        WriteInteger(settings.WhoCanMute);
        WriteInteger(settings.WhoCanKick);
        WriteInteger(settings.WhoCanBan);
        WriteBool(isOwner); // mute all button
        WriteInteger(chatSettings.ChatType); 
        WriteInteger(chatSettings.ChatWeight); 
        WriteInteger(chatSettings.ChatSpeed); 
        WriteInteger(chatSettings.ChatDistance); 
        WriteInteger(chatSettings.ChatProtection); 
    }
}