namespace Sadie.Game.Furniture;

public class FurnitureItem
{
    public FurnitureItem(
        int id, 
        string name, 
        string assetName, 
        char type, 
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
        Id = id;
        Name = name;
        AssetName = assetName;
        Type = type;
        AssetId = assetId;
        TileSpanX = tileSpanX;
        TileSpanY = tileSpanY;
        StackHeight = stackHeight;
        CanStack = canStack;
        CanWalk = canWalk;
        CanSit = canSit;
        CanLay = canLay;
        CanRecycle = canRecycle;
        CanTrade = canTrade;
        CanMarketplaceSell = canMarketplaceSell;
        CanInventoryStack = canInventoryStack;
        CanGift = canGift;
        InteractionType = interactionType;
        InteractionModes = interactionModes;
    }

    public int Id { get; }
    public string Name { get; }
    public string AssetName { get; }
    public char Type { get; }
    public int AssetId { get; }
    public int TileSpanX { get; }
    public int TileSpanY { get; }
    public double StackHeight { get; }
    public bool CanStack { get; }
    public bool CanWalk { get; }
    public bool CanSit { get; }
    public bool CanLay { get; }
    public bool CanRecycle { get; }
    public bool CanTrade { get; }
    public bool CanMarketplaceSell { get; }
    public bool CanInventoryStack { get; }
    public bool CanGift { get; }
    public string InteractionType { get; }
    public int InteractionModes { get; }
}