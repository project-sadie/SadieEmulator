namespace Sadie.Game.Players.Inventory;

public class PlayerInventoryRepository : IPlayerInventoryRepository
{
    private List<PlayerInventoryFurnitureItem> _items;

    public PlayerInventoryRepository(List<PlayerInventoryFurnitureItem> items)
    {
        _items = items;
    }

    public ICollection<PlayerInventoryFurnitureItem> Items => _items;
    
    public void AddItems(IEnumerable<PlayerInventoryFurnitureItem> items)
    {
        _items.AddRange(items);
    }
}