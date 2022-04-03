using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Moderation;

public class ModerationToolsWriter : NetworkPacketWriter
{
    public ModerationToolsWriter()
    {
        WriteShort(ServerPacketId.ModerationTools);
        WriteInt(0);
        WriteInt(0);
        WriteInt(0);
        WriteBoolean(true);
        WriteBoolean(true);
        WriteBoolean(true);
        WriteBoolean(true);
        WriteBoolean(true);
        WriteBoolean(true);
        WriteBoolean(true);
        WriteInt(0);
    }
}