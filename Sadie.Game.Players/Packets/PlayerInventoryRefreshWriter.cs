using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Game.Players.Packets;

public class PlayerInventoryRefreshWriter : NetworkPacketWriter
{
    public PlayerInventoryRefreshWriter()
    {
        WriteShort(ServerPacketId.PlayerInventoryRefresh);
    }
}