using Sadie.Game.Players;
using Sadie.Game.Players.Avatar;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerDataWriter : NetworkPacketWriter
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
        WriteString(playerData.LastOnline.ToString());
        WriteBoolean(false);
        WriteBoolean(false);
    }
}