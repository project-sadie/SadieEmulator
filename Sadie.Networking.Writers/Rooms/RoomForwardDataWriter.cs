using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomForwardDataWriter : NetworkPacketWriter
{
    public RoomForwardDataWriter(IRoom room, bool roomForward, bool unknown1)
    {
        WriteShort(ServerPacketId.RoomForwardData);
        WriteBoolean(unknown1);
        WriteLong(room.Id);
        WriteString(room.Name);
        WriteLong(room.OwnerId);
        WriteString(room.OwnerName);
        WriteInt(0); // state
        WriteInt(room.UserRepository.Count);
        WriteInt(room.MaxUsers);
        WriteString(room.Description);
        WriteInt(0); // trade mode?
        WriteInt(2); // unknown
        WriteInt(room.Score);
        WriteInt(0); // category
        WriteInt(room.Tags.Count);

        foreach (var tag in room.Tags)
        {
            WriteString(tag);
        }
        
        WriteInt(0 | 8); // TODO: base
        WriteBoolean(roomForward);
        WriteBoolean(false); // TODO: staff picked?
        WriteBoolean(false); // TODO: is group member?
        WriteBoolean(room.Settings.Muted);
        WriteInt(0); // TODO: mute option
        WriteInt(0); // TODO: kick option
        WriteInt(0); // TODO: ban option
        WriteBoolean(false); // TODO: current user has rights | gets mute all button
        WriteInt(0); // TODO: chat mode
        WriteInt(0); // TODO: chat weight
        WriteInt(0); // TODO: chat speed
        WriteInt(0); // TODO: chat distance
        WriteInt(0); // TODO: chat protection
    }
}