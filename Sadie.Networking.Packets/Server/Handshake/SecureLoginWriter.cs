namespace Sadie.Networking.Packets.Server.Handshake;

internal class SecureLoginWriter : NetworkPacketWriter
{
    internal SecureLoginWriter() : base(ServerPacketId.SecureLogin)
    {
    }
}