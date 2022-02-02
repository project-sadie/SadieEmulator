using Sadie.Game.Players;

namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerDataWriter : NetworkPacketWriter
{
    public PlayerDataWriter(IPlayerData playerData) : base(ServerPacketId.PlayerData)
    {
        WriteLong(playerData.Id);
        WriteString("admin");
        WriteString("hr-100.hd-190-7.ch-210-66.lg-270-82.sh-290-80");
        WriteString("M");
        WriteString("sadie4tw");
        WriteString("");
        WriteBoolean(false); // unknown1
        WriteInt(0); // respect points received
        WriteInt(0); // respect points
        WriteInt(0); // pet respect points
        WriteBoolean(false); // unknown2
        WriteString("01-01-1970 00:00:00"); // unknown3
        WriteBoolean(false); // allow username change
        WriteBoolean(false); // unknown4
    }
}