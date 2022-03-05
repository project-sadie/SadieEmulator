using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class NoobnessLevelWriter : NetworkPacketWriter
{
    public NoobnessLevelWriter(int level)
    {
        WriteShort(ServerPacketId.NoobnessLevel);
        WriteInt(level);
    }
}