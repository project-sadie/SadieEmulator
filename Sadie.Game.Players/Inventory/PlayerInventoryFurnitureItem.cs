using Sadie.Game.Furniture;

namespace Sadie.Game.Players.Inventory;

public class PlayerInventoryFurnitureItem(
    long id,
    FurnitureItem item,
    string metaData)
{
    public long Id { get; } = id;
    public FurnitureItem Item { get; } = item;
    public string MetaData { get; } = metaData;
}