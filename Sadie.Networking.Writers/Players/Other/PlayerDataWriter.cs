using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerDataWriter : NetworkPacketWriter
{
    public PlayerDataWriter(Player player)
    {
        WriteShort(ServerPacketId.PlayerData);
        WriteLong(player.Id);
        WriteString(player.Username);
        WriteString(player.AvatarData.FigureCode);
        WriteString(player.AvatarData.Gender == AvatarGender.Male ? "M" : "F");
        WriteString(player.AvatarData.Motto);
        WriteString(player.Username);
        WriteBool(false);
        WriteInteger(player.Respects.Count);
        WriteInteger(player.Data.RespectPoints);
        WriteInteger(player.Data.RespectPointsPet);
        WriteBool(false);
        WriteString(player.Data.LastOnline.ToString());
        WriteBool(false);
        WriteBool(false);
    }
}