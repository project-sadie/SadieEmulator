using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Furniture;

namespace Sadie.Game.Furniture;

public class FurnitureItemRepository(SadieContext dbContext)
{
    private Dictionary<int, FurnitureItemDto> _items = new();

    public async Task LoadInitialDataAsync()
    {
        _items = await dbContext.Set<FurnitureItemDto>()
            .ToDictionaryAsync(x => x.Id, x => x);
    }

    public Tuple<bool, FurnitureItemDto?> TryGetById(int itemId)
    {
        return new Tuple<bool, FurnitureItemDto?>(_items.TryGetValue(itemId, out var item), item);
    }
}