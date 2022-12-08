namespace Sadie.Game.Furniture;

public class FurnitureItemRepository
{
    private readonly FurnitureItemDao _furnitureItemDao;

    public FurnitureItemRepository(FurnitureItemDao furnitureItemDao)
    {
        _furnitureItemDao = furnitureItemDao;
    }

    public async Task LoadInitialDataAsync()
    {
        _items = await _furnitureItemDao.GetAllAsync();
    }

    private Dictionary<int, FurnitureItem> _items;

    public Tuple<bool, FurnitureItem?> TryGetById(int itemId)
    {
        return new Tuple<bool, FurnitureItem?>(_items.TryGetValue(itemId, out var item), item);
    }
}