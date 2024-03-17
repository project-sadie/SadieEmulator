namespace Sadie.Game.Furniture;

public class FurnitureItem(
    int id,
    string name,
    string assetName,
    FurnitureItemType type,
    int assetId,
    int tileSpanX,
    int tileSpanY,
    double stackHeight,
    bool canStack,
    bool canWalk,
    bool canSit,
    bool canLay,
    bool canRecycle,
    bool canTrade,
    bool canMarketplaceSell,
    bool canInventoryStack,
    bool canGift,
    string interactionType,
    int interactionModes)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public string AssetName { get; } = assetName;
    public FurnitureItemType Type { get; } = type;
    public int AssetId { get; } = assetId;
    public int TileSpanX { get; } = tileSpanX;
    public int TileSpanY { get; } = tileSpanY;
    public double StackHeight { get; } = stackHeight;
    public bool CanStack { get; } = canStack;
    public bool CanWalk { get; } = canWalk;
    public bool CanSit { get; } = canSit;
    public bool CanLay { get; } = canLay;
    public bool CanRecycle { get; } = canRecycle;
    public bool CanTrade { get; } = canTrade;
    public bool CanMarketplaceSell { get; } = canMarketplaceSell;
    public bool CanInventoryStack { get; } = canInventoryStack;
    public bool CanGift { get; } = canGift;
    public string InteractionType { get; } = interactionType;
    public int InteractionModes { get; } = interactionModes;
}