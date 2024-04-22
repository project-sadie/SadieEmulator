using Sadie.Database.Models.Catalog.Items;
using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Furniture;

public class FurnitureItem
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public string? AssetName { get; init; }
    public FurnitureItemType Type { get; init; }
    public int AssetId { get; init; }
    public int TileSpanX { get; init; }
    public int TileSpanY { get; init; }
    public double StackHeight { get; init; }
    public bool CanStack { get; init; }
    public bool CanWalk { get; init; }
    public bool CanSit { get; init; }
    public bool CanLay { get; init; }
    public bool CanRecycle { get; init; } 
    public bool CanTrade { get; init; }
    public bool CanMarketplaceSell { get; init; }
    public bool CanInventoryStack { get; init; }
    public bool CanGift { get; init; }
    public string? InteractionType { get; init; }
    public int InteractionModes { get; init; }
    public ICollection<CatalogItem> CatalogItems { get; init; } = [];
}