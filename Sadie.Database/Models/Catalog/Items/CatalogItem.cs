using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Catalog.Items;

public class CatalogItem
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public int CostCredits { get; init; }
    public int CostPoints { get; init; }
    public int CostPointsType { get; init; }
    public ICollection<FurnitureItem> FurnitureItems { get; init; } = [];
    public bool RequiresClubMembership { get; init; }
    public string? MetaData { get; init; }
    public int Amount { get; init; }
    public int SellLimit { get; init; }
}