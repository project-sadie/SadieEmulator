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
        WriteString("");
        WriteBoolean(false); // unknown1
        WriteInt(0); // respect points received
        WriteInt(0); // respect points
        WriteInt(0); // pet respect points
        WriteBoolean(false); // unknown2
        WriteString("01-01-1970 00:00:00"); // unknown3
        WriteBoolean(true); // allow username change
        WriteBoolean(false); // unknown4
    }
}