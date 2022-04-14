using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Moderation;

public class ModerationToolsWriter : NetworkPacketWriter
{
    public ModerationToolsWriter()
    {
        WriteShort(ServerPacketId.ModerationTools);
        WriteInteger(0);
        WriteInteger(0);
        WriteInteger(0);
        WriteBool(true);
        WriteBool(true);
        WriteBool(true);
        WriteBool(true);
        WriteBool(true);
        WriteBool(true);
        WriteBool(true);
        WriteInteger(0);
    }
}