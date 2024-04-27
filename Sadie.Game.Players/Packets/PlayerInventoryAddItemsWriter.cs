using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerInventoryAddItems)]
public class PlayerInventoryAddItemsWriter : AbstractPacketWriter
{
    public required List<PlayerFurnitureItem> Items { get; set; }
}