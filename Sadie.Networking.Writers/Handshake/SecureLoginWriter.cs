using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class SecureLoginWriter : NetworkPacketWriter
{
    public SecureLoginWriter()
    {
        WriteShort(ServerPacketId.SecureLogin);
    }
}