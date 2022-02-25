using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class SecureLoginWriter : NetworkPacketWriter
{
    public SecureLoginWriter() : base(ServerPacketId.SecureLogin)
    {
    }
}