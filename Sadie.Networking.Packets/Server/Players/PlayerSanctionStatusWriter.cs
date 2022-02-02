namespace Sadie.Networking.Packets.Server.Players;

public class PlayerSanctionStatusWriter : NetworkPacketWriter
{
    public PlayerSanctionStatusWriter() : base(ServerPacketId.PlayerSanctionStatus)
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