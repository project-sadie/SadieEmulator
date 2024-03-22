using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Shared;

public class PlayerAlertWriter : NetworkPacketWriter
{
    public PlayerAlertWriter(string message)
    {
        WriteShort(ServerPacketId.PlayerAlert);
        WriteString(message);
    }
}