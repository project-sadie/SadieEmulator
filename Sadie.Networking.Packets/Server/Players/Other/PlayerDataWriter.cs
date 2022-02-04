using Sadie.Game.Players;

namespace Sadie.Networking.Packets.Server.Players.Other;

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
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteBoolean(false);
        WriteString("01-01-1970 00:00:00");
        WriteBoolean(false);
        WriteBoolean(false);
    }
}