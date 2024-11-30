using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryRefresh)]
public class PlayerInventoryRefreshWriter : AbstractPacketWriter;