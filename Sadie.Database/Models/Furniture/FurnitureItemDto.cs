using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Furniture;

public class FurnitureItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AssetName { get; set; }
    public FurnitureItemType Type { get; set; }
    public int AssetId { get; set; }
    public int TileSpanX { get; set; }
    public int TileSpanY { get; set; }
    public double StackHeight { get; set; }
    public bool CanStack { get; set; }
    public bool CanWalk { get; set; }
    public bool CanSit { get; set; }
    public bool CanLay { get; set; }
    public bool CanRecycle { get; set; }
    public bool CanTrade { get; set; }
    public bool CanMarketplaceSell { get; set; }
    public bool CanInventoryStack { get; set; }
    public bool CanGift { get; set; }
    public string InteractionType { get; set; }
    public int InteractionModes { get; set; }
}