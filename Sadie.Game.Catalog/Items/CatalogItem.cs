using Sadie.Game.Furniture;

namespace Sadie.Game.Catalog;

public class CatalogItem
{
    public int Id { get; }
    public string Name { get; }
    public int CostCredits { get; }
    public int CostPoints { get; }
    public int CostPointsType { get; }
    public FurnitureItem FurnitureItem { get; }
    public bool RequiresClubMembership { get; }
    public string Metadata { get; }
    public int Amount { get; }
    public int SellLimit { get; }

    public CatalogItem(
        int id, 
        string name, 
        int costCredits, 
        int costPoints, 
        int costPointsType, 
        FurnitureItem furnitureItem, 
        bool requiresClubMembership, 
        string metadata, 
        int amount, 
        int sellLimit
    )
    {
        Id = id;
        Name = name;
        CostCredits = costCredits;
        CostPoints = costPoints;
        CostPointsType = costPointsType;
        FurnitureItem = furnitureItem;
        RequiresClubMembership = requiresClubMembership;
        Metadata = metadata;
        Amount = amount;
        SellLimit = sellLimit;
    }
}