using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Handshake;

public class SecureLoginWriter : AbstractPacketWriter
{
    public SecureLoginWriter()
    {
        WriteShort(ServerPacketId.SecureLogin);
    }
}