namespace Sadie.Networking.Packets.Server.Handshake;

public class SecureLoginOkPacket : NetworkPacketWriter
{
    public SecureLoginOkPacket() : base(ServerPacketIds.SecureLoginOk)
    {
    }
}