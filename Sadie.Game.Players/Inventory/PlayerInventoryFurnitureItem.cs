using Sadie.Game.Furniture;

namespace Sadie.Game.Players.Inventory;

public class PlayerInventoryFurnitureItem(
    long id,
    FurnitureItem item,
    string limitedData,
    string metaData,
    DateTime createdAt)
{
    public long Id { get; set; } = id;
    public FurnitureItem Item { get; } = item;
    public string LimitedData { get; } = limitedData;
    public string MetaData { get; } = metaData;
    public DateTime Created { get; } = createdAt;
}