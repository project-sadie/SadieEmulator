using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Furniture;

public class FurnitureItemFactory
{
    private readonly IServiceProvider _serviceProvider;

    public FurnitureItemFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public FurnitureItem Create(
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
        return ActivatorUtilities.CreateInstance<FurnitureItem>(
            _serviceProvider, 
            id, 
            name,
            assetName,
            (FurnitureItemType) type,
            assetId,
            tileSpanX,
            tileSpanY,
            stackHeight,
            canStack,
            canWalk,
            canSit,
            canLay,
            canRecycle,
            canTrade,
            canMarketplaceSell,
            canInventoryStack,
            canGift,
            interactionType,
            interactionModes);
    }
}