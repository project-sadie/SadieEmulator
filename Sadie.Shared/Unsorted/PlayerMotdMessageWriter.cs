using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Shared.Unsorted;

public class PlayerMotdMessageWriter : NetworkPacketWriter
{
    public PlayerMotdMessageWriter(List<string> paragraphs)
    {
        WriteShort(ServerPacketId.PlayerMotdMessage);
        WriteInteger(paragraphs.Count);

        foreach (var message in paragraphs)
        {
            WriteString(message);
        }
    }
}