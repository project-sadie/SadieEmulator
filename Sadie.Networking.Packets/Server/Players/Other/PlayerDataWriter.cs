using System.Globalization;
using Sadie.Game.Players;

namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerDataWriter : NetworkPacketWriter // @hardcode
{
    public PlayerDataWriter(IPlayerData playerData) : base(ServerPacketId.PlayerData)
    {
        WriteLong(playerData.Id);
        WriteString(playerData.Username);
        WriteString(playerData.FigureCode);
        WriteString(playerData.Gender == PlayerAvatarGender.Male ? "M" : "F");
        WriteString(playerData.Motto);
        WriteString(playerData.Username);
        WriteBoolean(false);
        WriteLong(playerData.RespectsReceived);
        WriteLong(playerData.RespectPoints);
        WriteLong(playerData.RespectPointsPet);
        WriteBoolean(false);
        WriteString(playerData.LastOnline.ToString(CultureInfo.InvariantCulture));
        WriteBoolean(false);
        WriteBoolean(false);
    }
}