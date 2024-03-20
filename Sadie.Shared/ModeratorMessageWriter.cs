using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Shared;

public class ModeratorMessageWriter : NetworkPacketWriter
{
    public ModeratorMessageWriter(string message, string link)
    {
        WriteShort(ServerPacketId.ModeratorMessage);
        WriteString(message);
        WriteString(link);
    }
}