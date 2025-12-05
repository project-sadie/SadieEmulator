using Sadie.API;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Rooms;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Users;

[PacketId(ServerPacketId.RoomForwardData)]
public class RoomForwardDataWriter : AbstractPacketWriter
{
    public required RoomDto Room { get; init; }
    public required bool RoomForward { get; init; }
    public required bool EnterRoom { get; init; }
    public required bool IsOwner { get; init; }
    public required int UsersNow { get; init; }
    public required IPlayerRepository PlayerRepository { get; init; }

    public override async void OnSerialize(INetworkPacketWriter writer)
    {
        var settings = Room.Settings;
        var chatSettings = Room.ChatSettings;
        
        var owner = await PlayerRepository.GetPlayerByIdAsyncT(Room.OwnerId);

        writer.WriteBool(EnterRoom);
        writer.WriteLong(Room.Id);
        writer.WriteString(Room.Name);
        writer.WriteLong(Room.OwnerId);
        writer.WriteString(owner?.Username ?? string.Empty);
        writer.WriteInteger((int) Room.Settings.AccessType);
        writer.WriteInteger(UsersNow);
        writer.WriteInteger(Room.MaxUsersAllowed);
        writer.WriteString(Room.Description);
        writer.WriteInteger((int) settings.TradeOption);
        writer.WriteInteger(Room.PlayerLikes.Count);
        writer.WriteInteger(1); // ranking
        writer.WriteInteger(0); // category
        writer.WriteInteger(Room.Tags.Count);

        foreach (var tag in Room.Tags)
        {
            writer.WriteString(tag.Name);
        }
        
        writer.WriteInteger((int) RoomBitmask.ShowOwner);
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