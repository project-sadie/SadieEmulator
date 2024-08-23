using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Packets.Writers.Furniture;

[PacketId(ServerPacketId.WiredEffect)]
public class WiredEffectWriter : AbstractPacketWriter
{
    public required bool StuffTypeSelectionEnabled { get; init; }
    public required int MaxItemsSelected { get; init; }
    public required ICollection<PlayerFurnitureItemPlacementData> SelectedItems { get; init; }
    public required PlayerFurnitureItem Item { get; init; }
    
    public override void OnSerialize(NetworkPacketWriter writer)
    {
        writer.WriteBool(StuffTypeSelectionEnabled);
        writer.WriteInteger(MaxItemsSelected);
        
        writer.WriteInteger(SelectedItems.Count);
        
        foreach (var item in SelectedItems)
        {
            writer.WriteInteger(item.Id);
        }
        
        writer.WriteInteger(Item.FurnitureItem.AssetId);
        writer.WriteInteger(Item.Id);
        writer.WriteString(Item.MetaData);
        
        writer.WriteInteger(0);
        writer.WriteInteger(0);
        writer.WriteInteger(7);
        writer.WriteInteger(0);
        writer.WriteInteger(0);
    }
}