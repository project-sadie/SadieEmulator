namespace Sadie.Networking.Packets.Server.GameCentre;

public class GameCenterConfigWriter : NetworkPacketWriter
{
    public GameCenterConfigWriter() : base(ServerPacketId.GameCentreConfig)
    {
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteInt(3);
        WriteInt(1);
        WriteInt(1);
        WriteString("BaseJumpBigParachute");
        WriteInt(1);
    }
}