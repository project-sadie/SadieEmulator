using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryRefreshWriter : NetworkPacketWriter
{
    public PlayerInventoryRefreshWriter()
    {
        WriteShort(ServerPacketId.PlayerInventoryRefresh);
    }
}