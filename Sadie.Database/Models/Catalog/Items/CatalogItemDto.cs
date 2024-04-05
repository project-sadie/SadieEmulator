using Sadie.Database.Models.Furniture;

namespace Sadie.Database.Models.Catalog.Items;

public class CatalogItemDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int CostCredits { get; set; }
    public int CostPoints { get; set; }
    public int CostPointsType { get; set; }
    public List<FurnitureItemDto> FurnitureItems { get; set; }
    public bool RequiresClubMembership { get; set; }
    public string Metadata { get; set; }
    public long CatalogPageId { get; set; }
    public int Amount { get; set; }
    public int SellLimit { get; set; }
}