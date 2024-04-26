using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Shared.Unsorted;

public class PlayerAlertWriter : AbstractPacketWriter
{
    public PlayerAlertWriter(string message)
    {
        WriteShort(ServerPacketId.PlayerAlert);
        WriteString(message);
    }
}