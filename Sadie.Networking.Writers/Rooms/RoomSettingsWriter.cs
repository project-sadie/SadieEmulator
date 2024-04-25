using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Rooms;

public class RoomSettingsWriter : NetworkPacketWriter
{
    public RoomSettingsWriter(RoomLogic roomData)
    {
        WriteShort(ServerPacketId.RoomSettings);
        WriteInteger(roomData.Id);
        WriteString(roomData.Name);
        WriteString(roomData.Description);
        WriteInteger((int) roomData.Settings.AccessType);
        WriteInteger(0); // TODO: category
        WriteInteger(roomData.MaxUsersAllowed);
        WriteInteger(roomData.MaxUsersAllowed);
        WriteInteger(roomData.Tags.Count);

        foreach (var tag in roomData.Tags)
        {
            WriteString(tag.Name);
        }

        var settings = roomData.Settings;
        var chatSettings = roomData.ChatSettings;
        
        WriteInteger(settings.TradeOption);
        WriteInteger(settings.AllowPets ? 1 : 0);
        WriteInteger(settings.CanPetsEat ? 1 : 0);
        WriteInteger(settings.CanUsersOverlap ? 1 : 0);
        WriteInteger(settings.HideWalls ? 1 : 0);
        WriteInteger(settings.WallThickness);
        WriteInteger(settings.FloorThickness);
        WriteInteger(chatSettings.ChatType); 
        WriteInteger(chatSettings.ChatWeight); 
        WriteInteger(chatSettings.ChatSpeed); 
        WriteInteger(chatSettings.ChatDistance); 
        WriteInteger(chatSettings.ChatProtection); 
        WriteBool(false); // TODO: unknown
        WriteInteger(settings.WhoCanMute);
        WriteInteger(settings.WhoCanKick);
        WriteInteger(settings.WhoCanBan);
    }
}