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
        WriteString(playerData.LastOnline.ToString());
        WriteBoolean(false);
        WriteBoolean(false);
    }
}