using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Shared.Unsorted;

public class PlayerMotdMessageWriter : NetworkPacketWriter
{
    public PlayerMotdMessageWriter(List<string?> paragraphs)
    {
        WriteShort(ServerPacketId.PlayerMotdMessage);
        WriteInteger(paragraphs.Count);

        foreach (var message in paragraphs)
        {
            WriteString(message);
        }
    }
}