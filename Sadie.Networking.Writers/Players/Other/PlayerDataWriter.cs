using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Networking;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerData)]
public class PlayerDataWriter : AbstractPacketWriter
{
    public required IPlayerLogic Player { get; init; }
    
    public override void OnSerialize(INetworkPacketWriter writer)
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
        writer.WriteString(Player.Data.LastOnline!.Value.ToString("dd-MM-yyyy HH:mm:ss"));
        writer.WriteBool(false);
        writer.WriteBool(false);
    }
}