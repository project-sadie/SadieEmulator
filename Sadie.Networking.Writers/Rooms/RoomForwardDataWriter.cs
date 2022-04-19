using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomForwardDataWriter : NetworkPacketWriter
{
    public RoomForwardDataWriter(IRoomData room, bool roomForward, bool unknown1)
    {
        WriteShort(ServerPacketId.RoomForwardData);
        WriteBool(unknown1);
        WriteLong(room.Id);
        WriteString(room.Name);
        WriteLong(room.OwnerId);
        WriteString(room.OwnerName);
        WriteInteger((int) room.Settings.AccessType);
        WriteInteger(room.UserRepository.Count);
        WriteInteger(room.MaxUsers);
        WriteString(room.Description);
        WriteInteger(0); // trade mode?
        WriteInteger(2); // unknown
        WriteInteger(room.Score);
        WriteInteger(0); // category
        WriteInteger(room.Tags.Count);

        foreach (var tag in room.Tags)
        {
            WriteString(tag);
        }

        var settings = room.Settings;
        
        WriteInteger(0 | 8); // TODO: base
        WriteBool(roomForward);
        WriteBool(false); // TODO: staff picked?
        WriteBool(false); // TODO: is group member?
        WriteBool(room.Muted);
        WriteInteger(settings.WhoCanMute);
        WriteInteger(settings.WhoCanKick);
        WriteInteger(settings.WhoCanBan);
        WriteBool(false); // TODO: current user has rights | gets mute all button
        WriteInteger(0); // TODO: chat mode
        WriteInteger(0); // TODO: chat weight
        WriteInteger(0); // TODO: chat speed
        WriteInteger(0); // TODO: chat distance
        WriteInteger(0); // TODO: chat protection
    }
}