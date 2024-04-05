using Sadie.Database.Models.Furniture;
using Sadie.Game.Furniture;

namespace Sadie.Game.Players.Inventory;

public class PlayerInventoryFurnitureItem(
    long id,
    FurnitureItemDto furnitureItem,
    string limitedData,
    string metaData,
    DateTime createdAt)
{
    public long Id { get; set; } = id;
    public FurnitureItemDto FurnitureItem { get; } = furnitureItem;
    public string LimitedData { get; } = limitedData;
    public string MetaData { get; } = metaData;
    public DateTime Created { get; } = createdAt;
}