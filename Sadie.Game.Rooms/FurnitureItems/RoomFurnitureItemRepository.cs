namespace Sadie.Game.Rooms.FurnitureItems;

public class RoomFurnitureItemRepository : IRoomFurnitureItemRepository
{
    private List<RoomFurnitureItem> _items;

    public RoomFurnitureItemRepository(List<RoomFurnitureItem> items)
    {
        _items = items;
    }

    public ICollection<RoomFurnitureItem> Items => _items;
    
    public void AddItems(IEnumerable<RoomFurnitureItem> items)
    {
        _items.AddRange(items);
    }

    public void RemoveItems(List<long> itemIds)
    {
        _items = _items.Where(x => !itemIds.Contains(x.Id)).ToList();
    }
}