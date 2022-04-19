using Sadie.Game.Rooms;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsWriter : NetworkPacketWriter
{
    public RoomSettingsWriter(IRoomData roomData)
    {
        WriteShort(ServerPacketId.RoomSettings);
        WriteInteger(roomData.Id);
        WriteString(roomData.Name);
        WriteString(roomData.Description);
        WriteInteger((int) roomData.Settings.AccessType);
        WriteInteger(0); // TODO: category
        WriteInteger(roomData.MaxUsers);
        WriteInteger(roomData.MaxUsers);
        WriteInteger(roomData.Tags.Count);

        foreach (var tag in roomData.Tags)
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