using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerInventoryRefresh)]
public class PlayerInventoryRefreshWriter : AbstractPacketWriter
{
}