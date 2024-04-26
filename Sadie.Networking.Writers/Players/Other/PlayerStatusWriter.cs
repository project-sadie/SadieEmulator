using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerStatusWriter : AbstractPacketWriter
{
    public PlayerStatusWriter(bool isOpen, bool isShuttingDown, bool isAuthentic)
    {
        WriteShort(ServerPacketId.PlayerStatus);
        WriteBool(isOpen);
        WriteBool(isShuttingDown);
        WriteBool(isAuthentic);
    }
}