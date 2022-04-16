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
        WriteInteger((int) room.Settings.AccessType);
        WriteInteger(0); // TODO: category
        WriteInteger(room.MaxUsers);
        WriteInteger(room.MaxUsers);
        WriteInteger(room.Tags.Count);

        foreach (var tag in room.Tags)
        {
            WriteString(tag);
        }
        
        WriteInteger(0); // TODO: trade mode
        WriteInteger(1); // TODO: allow pets
        WriteInteger(1); // TODO: allow pets eat
        WriteInteger(1); // TODO: allow walk through
        WriteInteger(0); // TODO: hiding wall
        WriteInteger(0); // TODO: wall size
        WriteInteger(0); // TODO: floor size
        WriteInteger(0); // TODO: chat mode
        WriteInteger(0); // TODO: chat weight
        WriteInteger(0); // TODO: chat speed
        WriteInteger(0); // TODO: chat distance
        WriteInteger(0); // TODO: chat protection
        WriteBool(false); // TODO: unknown
        WriteInteger(0); // TODO: mute option
        WriteInteger(0); // TODO: kick option
        WriteInteger(0); // TODO: ban option
    }
}