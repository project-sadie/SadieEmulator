using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Handshake;

public class NoobnessLevelWriter : AbstractPacketWriter
{
    public NoobnessLevelWriter(int level)
    {
        WriteShort(ServerPacketId.NoobnessLevel);
        WriteInteger(level);
    }
}