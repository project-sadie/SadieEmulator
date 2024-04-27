using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Shared.Unsorted;

public class ModeratorMessageWriter : AbstractPacketWriter
{
    public ModeratorMessageWriter(string message, string link)
    {
        WriteShort(ServerPacketId.ModeratorMessage);
        WriteString(message);
        WriteString(link);
    }
}