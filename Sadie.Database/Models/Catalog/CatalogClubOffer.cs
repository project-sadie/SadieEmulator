namespace Sadie.Database.Models.Catalog;

public class CatalogClubOffer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int DurationDays { get; set; }
    public int CostCredits { get; set; }
    public int CostPoints { get; set; }
    public int CostPointsType { get; set; }
    public bool IsVip { get; set; }
}