using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Furniture;

namespace Sadie.Game.Catalog.Items;

public class CatalogItemFactory(IServiceProvider serviceProvider)
{
    public CatalogItem Create(
        int id, 
        string name, 
        int costCredits, 
        int costPoints, 
        int costPointsType, 
        List<FurnitureItem> furnitureItems, 
        bool requiresClubMembership, 
        string metadata, 
        int amount, 
        int sellLimit)
    {
        return ActivatorUtilities.CreateInstance<CatalogItem>(
            serviceProvider, 
            id, 
            name, 
            costCredits,
            costPoints,
            costPointsType,
            furnitureItems,
            requiresClubMembership, 
            metadata, 
            amount, 
            sellLimit);
    }
}