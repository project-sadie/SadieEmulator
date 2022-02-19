namespace Sadie.Networking.Packets.Server.Handshake;

public class NoobnessLevelWriter : NetworkPacketWriter
{
    public NoobnessLevelWriter(int level) : base(ServerPacketId.NoobnessLevel)
    {
        WriteInt(level);
    }
}