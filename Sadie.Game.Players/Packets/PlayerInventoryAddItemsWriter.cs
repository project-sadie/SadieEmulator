using Sadie.Database.Models.Players;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerInventoryAddItems)]
public class PlayerInventoryAddItemsWriter : AbstractPacketWriter
{
    public required List<PlayerFurnitureItem> Items { get; set; }
}