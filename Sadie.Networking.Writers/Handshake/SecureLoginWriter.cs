using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class SecureLoginWriter : NetworkPacketWriter
{
    public SecureLoginWriter()
    {
        WriteShort(ServerPacketId.SecureLogin);
    }
}