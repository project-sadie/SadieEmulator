namespace Sadie.Networking.Packets.Server.Players;

public class PlayerSanctionStatus : NetworkPacketWriter
{
    public PlayerSanctionStatus() : base(ServerPacketIds.PlayerSanctionStatus)
    {
        WriteBoolean(false);
        WriteBoolean(false);
        WriteString("ALERT");
        WriteInt(0);
        WriteInt(30);
        WriteString("cfh.reason.EMPTY");
        WriteString(DateTime.Now.Year.ToString());
        WriteInt(0);
        WriteString("ALERT");
        WriteInt(0);
        WriteInt(30);
        WriteBoolean(false);
        WriteString("");
    }
}