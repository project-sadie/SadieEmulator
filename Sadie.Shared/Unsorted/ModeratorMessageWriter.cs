using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Shared.Unsorted;

public class ModeratorMessageWriter : NetworkPacketWriter
{
    public ModeratorMessageWriter(string message, string link)
    {
        WriteShort(ServerPacketId.ModeratorMessage);
        WriteString(message);
        WriteString(link);
    }
}