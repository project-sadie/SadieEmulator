using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomForwardDataWriter : NetworkPacketWriter
{
    public RoomForwardDataWriter(Room room, bool roomForward, bool enterRoom, bool isOwner)
    {
        var settings = room.Settings;
        
        WriteShort(ServerPacketId.RoomForwardData);
        WriteBool(enterRoom);
        WriteLong(room.Id);
        WriteString(room.Name);
        WriteLong(room.OwnerId);
        WriteString(room.OwnerName);
        WriteInteger((int) room.Settings.AccessType);
        WriteInteger(room.UserRepository.Count);
        WriteInteger(room.MaxUsers);
        WriteString(room.Description);
        WriteInteger(settings.TradeOption);
        WriteInteger(2); // unknown
        WriteInteger(room.PlayerLikes.Count);
        WriteInteger(0); // category
        WriteInteger(room.Tags.Count);

        foreach (var tag in room.Tags)
        {
            WriteString(tag);
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
        WriteInteger(settings.ChatType); 
        WriteInteger(settings.ChatWeight); 
        WriteInteger(settings.ChatSpeed); 
        WriteInteger(settings.ChatDistance); 
        WriteInteger(settings.ChatProtection); 
    }
}