using Sadie.API;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomForwardData)]
public class RoomForwardDataWriter : AbstractPacketWriter
{
    public required IRoomLogic Room { get; init; }
    public required bool RoomForward { get; init; }
    public required bool EnterRoom { get; init; }
    public required bool IsOwner { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        var settings = Room.Settings;
        var chatSettings = Room.ChatSettings;
        
        writer.WriteBool(EnterRoom);
        writer.WriteLong(Room.Id);
        writer.WriteString(Room.Name);
        writer.WriteLong(Room.OwnerId);
        writer.WriteString(Room.Owner.Username);
        writer.WriteInteger((int) Room.Settings.AccessType);
        writer.WriteInteger(Room.UserRepository.Count);
        writer.WriteInteger(Room.MaxUsersAllowed);
        writer.WriteString(Room.Description);
        writer.WriteInteger((int) settings.TradeOption);
        writer.WriteInteger(2); // unknown
        writer.WriteInteger(Room.PlayerLikes.Count);
        writer.WriteInteger(0); // category
        writer.WriteInteger(Room.Tags.Count);

        foreach (var tag in Room.Tags)
        {
            writer.WriteString(tag.Name);
        }
        
        writer.WriteInteger(0 | 8);
        writer.WriteBool(RoomForward);
        writer.WriteBool(false);
        writer.WriteBool(false);
        writer.WriteBool(Room.IsMuted);
        writer.WriteInteger(settings.WhoCanMute);
        writer.WriteInteger(settings.WhoCanKick);
        writer.WriteInteger(settings.WhoCanBan);
        writer.WriteBool(IsOwner);
        writer.WriteInteger(chatSettings.ChatType); 
        writer.WriteInteger(chatSettings.ChatWeight); 
        writer.WriteInteger(chatSettings.ChatSpeed); 
        writer.WriteInteger(chatSettings.ChatDistance); 
        writer.WriteInteger(chatSettings.ChatProtection); 
    }
}