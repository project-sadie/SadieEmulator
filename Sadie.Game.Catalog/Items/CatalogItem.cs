using Sadie.Game.Furniture;

namespace Sadie.Game.Catalog.Items;

public class CatalogItem(
    int id,
    string name,
    int costCredits,
    int costPoints,
    int costPointsType,
    List<FurnitureItem> furnitureItems,
    bool requiresClubMembership,
    string metadata,
    long catalogPageId,
    int amount,
    int sellLimit)
{
    public int Id { get; } = id;
    public string Name { get; } = name;
    public int CostCredits { get; } = costCredits;
    public int CostPoints { get; } = costPoints;
    public int CostPointsType { get; } = costPointsType;
    public List<FurnitureItem> FurnitureItems { get; } = furnitureItems;
    public bool RequiresClubMembership { get; } = requiresClubMembership;
    public string Metadata { get; } = metadata;
    public long CatalogPageId { get; } = catalogPageId;
    public int Amount { get; } = amount;
    public int SellLimit { get; } = sellLimit;
}