namespace Sadie.Networking.Packets.Server.Handshake;

public class SecureLoginWriter : NetworkPacketWriter
{
    public SecureLoginWriter() : base(ServerPacketId.SecureLogin)
    {
    }
}