using Sadie.API;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerData)]
public class PlayerDataWriter : AbstractPacketWriter
{
    public required IPlayerLogic Player { get; init; }
    
    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteLong(Player.Player.Id);
        writer.WriteString(Player.Player.Username);
        writer.WriteString(Player.Player.AvatarData!.FigureCode);
        writer.WriteString(Player.Player.AvatarData.Gender == PlayerAvatarGender.Male ? "M" : "F");
        writer.WriteString(Player.Player.AvatarData.Motto ?? string.Empty);
        writer.WriteString(Player.Player.Username);
        writer.WriteBool(false);
        writer.WriteInteger(Player.Player.Respects.Count);
        writer.WriteInteger(Player.Player.Data.RespectPoints);
        writer.WriteInteger(Player.Player.Data.RespectPointsPet);
        writer.WriteBool(false);
        writer.WriteString(Player.Player.Data.LastOnline!.Value.ToString("dd-MM-yyyy HH:mm:ss"));
        writer.WriteBool(false);
        writer.WriteBool(false);
    }
}