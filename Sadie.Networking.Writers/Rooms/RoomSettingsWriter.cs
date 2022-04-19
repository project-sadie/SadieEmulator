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

        var settings = roomData.Settings;
        
        WriteInteger(0); // TODO: trade mode
        WriteInteger(settings.AllowPets ? 1 : 0);
        WriteInteger(settings.CanPetsEat ? 1 : 0);
        WriteInteger(settings.CanUsersOverlap ? 1 : 0); // TODO: allow walk through
        WriteInteger(settings.HideWalls ? 1 : 0);
        WriteInteger(0); // TODO: wall size
        WriteInteger(0); // TODO: floor size
        WriteInteger(0); // TODO: chat mode
        WriteInteger(0); // TODO: chat weight
        WriteInteger(0); // TODO: chat speed
        WriteInteger(0); // TODO: chat distance
        WriteInteger(0); // TODO: chat protection
        WriteBool(false); // TODO: unknown
        WriteInteger(settings.WhoCanMute);
        WriteInteger(settings.WhoCanKick);
        WriteInteger(settings.WhoCanBan);
    }
}