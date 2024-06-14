using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players.Packets;

[PacketId(ServerPacketId.PlayerInventoryAddItems)]
public class PlayerInventoryAddItemsWriter : AbstractPacketWriter
{
    public required List<PlayerFurnitureItem> FurnitureItems { get; set; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteInteger(FurnitureItems.Count);
        writer.WriteInteger(1);
        writer.WriteInteger(FurnitureItems.Count);

        foreach (var item in FurnitureItems)
        {
            writer.WriteInteger(item.Id);
        }
    }
}