using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Inventory;

public class PlayerInventoryRefreshWriter : NetworkPacketWriter
{
    public PlayerInventoryRefreshWriter()
    {
        WriteShort(ServerPacketId.PlayerInventoryRefresh);
    }
}