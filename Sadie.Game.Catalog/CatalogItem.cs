namespace Sadie.Game.Catalog;

public class CatalogItem
{
    public int Id { get; }
    public string Name { get; }
    public int CostCredits { get; }
    public int CostPoints { get; }
    public int CostPointsType { get; }
    public bool CanGift { get; }
    public bool RequiresClubMembership { get; }
    public int Amount { get; }
    public int SellLimit { get; }

    public CatalogItem(int id, string name, int costCredits, int costPoints, int costPointsType, bool canGift, bool requiresClubMembership, int amount, int sellLimit)
    {
        Id = id;
        Name = name;
        CostCredits = costCredits;
        CostPoints = costPoints;
        CostPointsType = costPointsType;
        CanGift = canGift;
        RequiresClubMembership = requiresClubMembership;
        Amount = amount;
        SellLimit = sellLimit;
    }
}