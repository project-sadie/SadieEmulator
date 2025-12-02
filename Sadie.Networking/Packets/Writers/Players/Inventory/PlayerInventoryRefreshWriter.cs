using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryRefresh)]
public class PlayerInventoryRefreshWriter : AbstractPacketWriter;