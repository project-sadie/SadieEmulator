using Sadie.API;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomSettings)]
public class RoomSettingsWriter : AbstractPacketWriter
{
    public required IRoomLogic Room { get; init; }
    
    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Room.Id);
        writer.WriteString(Room.Name);
        writer.WriteString(Room.Description);
        writer.WriteInteger((int) Room.Settings.AccessType);
        writer.WriteInteger(0);
        writer.WriteInteger(Room.MaxUsersAllowed);
        writer.WriteInteger(Room.MaxUsersAllowed);
        writer.WriteInteger(Room.Tags.Count);

        foreach (var tag in Room.Tags)
        {
            writer.WriteString(tag.Name);
        }

        var settings = Room.Settings;
        var chatSettings = Room.ChatSettings;
        
        writer.WriteInteger((int) settings.TradeOption);
        writer.WriteInteger(settings.AllowPets ? 1 : 0);
        writer.WriteInteger(settings.CanPetsEat ? 1 : 0);
        writer.WriteInteger(settings.CanUsersOverlap ? 1 : 0);
        writer.WriteInteger(settings.HideWalls ? 1 : 0);
        writer.WriteInteger(settings.WallThickness);
        writer.WriteInteger(settings.FloorThickness);
        writer.WriteInteger(chatSettings.ChatType); 
        writer.WriteInteger(chatSettings.ChatWeight); 
        writer.WriteInteger(chatSettings.ChatSpeed); 
        writer.WriteInteger(chatSettings.ChatDistance); 
        writer.WriteInteger(chatSettings.ChatProtection); 
        writer.WriteBool(false);
        writer.WriteInteger(settings.WhoCanMute);
        writer.WriteInteger(settings.WhoCanKick);
        writer.WriteInteger(settings.WhoCanBan);
    }
}