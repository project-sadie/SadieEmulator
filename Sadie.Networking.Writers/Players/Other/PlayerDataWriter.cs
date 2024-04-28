using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerData)]
public class PlayerDataWriter : AbstractPacketWriter
{
    public required Player Player { get; init; }
    
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteLong(Player.Id);
        writer.WriteString(Player.Username);
        writer.WriteString(Player.AvatarData.FigureCode);
        writer.WriteString(Player.AvatarData.Gender == AvatarGender.Male ? "M" : "F");
        writer.WriteString(Player.AvatarData.Motto);
        writer.WriteString(Player.Username);
        writer.WriteBool(false);
        writer.WriteInteger(Player.Respects.Count);
        writer.WriteInteger(Player.Data.RespectPoints);
        writer.WriteInteger(Player.Data.RespectPointsPet);
        writer.WriteBool(false);
        writer.WriteString(Player.Data.LastOnline.ToString());
        writer.WriteBool(false);
        writer.WriteBool(false);
    }
}