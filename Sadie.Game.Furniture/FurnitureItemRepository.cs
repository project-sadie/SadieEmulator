using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Furniture;

namespace Sadie.Game.Furniture;

public class FurnitureItemRepository(SadieContext dbContext)
{
    private Dictionary<int, FurnitureItem> _items = new();

    public async Task LoadInitialDataAsync()
    {
        _items = await dbContext.Set<FurnitureItem>()
            .ToDictionaryAsync(x => x.Id, x => x);
    }

    public Tuple<bool, FurnitureItem?> TryGetById(int itemId)
    {
        return new Tuple<bool, FurnitureItem?>(_items.TryGetValue(itemId, out var item), item);
    }
}