using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerStatusWriter : NetworkPacketWriter
{
    public PlayerStatusWriter(bool isOpen, bool isShuttingDown, bool isAuthentic)
    {
        WriteShort(ServerPacketId.PlayerStatus);
        WriteBoolean(isOpen);
        WriteBoolean(isShuttingDown);
        WriteBoolean(isAuthentic);
    }
}