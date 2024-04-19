namespace Sadie.Database.Models.Catalog;

public class CatalogClubOffer
{
    public int Id { get; init; }
    public string Name { get; init; }
    public int DurationDays { get; init; }
    public int CostCredits { get; init; }
    public int CostPoints { get; init; }
    public int CostPointsType { get; init; }
    public bool IsVip { get; init; }
}