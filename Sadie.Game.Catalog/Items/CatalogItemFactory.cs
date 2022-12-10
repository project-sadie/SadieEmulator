using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Furniture;

namespace Sadie.Game.Catalog;

public class CatalogItemFactory
{
    private readonly IServiceProvider _serviceProvider;

    public CatalogItemFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public CatalogItem Create(int id, 
        string name, 
        int costCredits, 
        int costPoints, 
        int costPointsType, 
        FurnitureItem furnitureItem, 
        bool requiresClubMembership, string metadata, int amount, int sellLimit)
    {
        return ActivatorUtilities.CreateInstance<CatalogItem>(
            _serviceProvider, 
            id, 
            name, 
            costCredits,
            costPoints,
            costPointsType,
            furnitureItem,
            requiresClubMembership, 
            metadata, 
            amount, 
            sellLimit);
    }
}