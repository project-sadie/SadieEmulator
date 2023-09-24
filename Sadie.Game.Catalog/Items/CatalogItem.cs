using Sadie.Game.Furniture;

namespace Sadie.Game.Catalog.Items;

public class CatalogItem
{
    public int Id { get; }
    public string Name { get; }
    public int CostCredits { get; }
    public int CostPoints { get; }
    public int CostPointsType { get; }
    public List<FurnitureItem> FurnitureItems { get; }
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
        List<FurnitureItem> furnitureItems, 
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
        FurnitureItems = furnitureItems;
        RequiresClubMembership = requiresClubMembership;
        Metadata = metadata;
        Amount = amount;
        SellLimit = sellLimit;
    }
}