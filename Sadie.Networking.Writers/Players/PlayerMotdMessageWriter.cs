using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players;

public class PlayerMotdMessageWriter : NetworkPacketWriter
{
    public PlayerMotdMessageWriter(List<string> paragraphs)
    {
        WriteShort(ServerPacketId.PlayerMessage);
        WriteInt(paragraphs.Count);

        foreach (var message in paragraphs)
        {
            WriteString(message);
        }
    }
}