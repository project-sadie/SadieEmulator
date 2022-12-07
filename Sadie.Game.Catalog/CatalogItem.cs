namespace Sadie.Game.Catalog;

public class CatalogItem
{
    public int Id { get; }
    public string Name { get; }
    public int CostCredits { get; }
    
    public CatalogItem(int id, string name, int costCredits)
    {
        Id = id;
        Name = name;
        CostCredits = costCredits;
    }
}