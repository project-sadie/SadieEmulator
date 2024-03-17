namespace Sadie.Game.Furniture;

public class FurnitureItemRepository(FurnitureItemDao furnitureItemDao)
{
    public async Task LoadInitialDataAsync()
    {
        _items = await furnitureItemDao.GetAllAsync();
    }

    private Dictionary<int, FurnitureItem> _items;

    public Tuple<bool, FurnitureItem?> TryGetById(int itemId)
    {
        return new Tuple<bool, FurnitureItem?>(_items.TryGetValue(itemId, out var item), item);
    }
}