using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsWriter : NetworkPacketWriter
{
    public RoomSettingsWriter(IRoom room)
    {
        WriteShort(ServerPacketId.RoomSettings);
        WriteInteger(room.Id);
        WriteString(room.Name);
        WriteString(room.Description);
        WriteInteger(0); // locked/open/etc
        WriteInteger(0); // category
        WriteInteger(room.MaxUsers);
        WriteInteger(room.MaxUsers);
        WriteInteger(room.Tags.Count);

        foreach (var tag in room.Tags)
        {
            WriteString(tag);
        }
        
        WriteInteger(0); // trade mode
        WriteInteger(1); // allow pets
        WriteInteger(1); // allow pets eat
        WriteInteger(1); // allow walk through
        WriteInteger(0); // hiding wall
        WriteInteger(0); // wall size
        WriteInteger(0); // floor size
        WriteInteger(0); // chat mode
        WriteInteger(0); // chat weight
        WriteInteger(0); // chat speed
        WriteInteger(0); // chat distance
        WriteInteger(0); // chat protection
        WriteBool(false); // unknown
        WriteInteger(0); // mute option
        WriteInteger(0); // kick option
        WriteInteger(0); // ban option
    }
}