using Microsoft.Extensions.DependencyInjection;
using Sadie.Shared.Helpers;

namespace Sadie.Game.Furniture;

public class FurnitureItemFactory(IServiceProvider serviceProvider)
{
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
            serviceProvider, 
            id, 
            name,
            assetName,
            EnumHelpers.GetEnumValueFromDescription<FurnitureItemType>(type.ToString()),
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