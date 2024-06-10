using Sadie.Networking.Serialization;

namespace Sadie.Shared.Unsorted;

public class PlayerMotdMessageWriter : NetworkPacketWriter
{
    public PlayerMotdMessageWriter(List<string?> paragraphs)
    {
        WriteInteger(paragraphs.Count);

        foreach (var message in paragraphs)
        {
            WriteString(message);
        }
    }
}