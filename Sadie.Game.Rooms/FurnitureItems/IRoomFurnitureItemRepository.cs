namespace Sadie.Game.Rooms.FurnitureItems;

public interface IRoomFurnitureItemRepository
{
    ICollection<RoomFurnitureItem> Items { get; }
    void AddItems(IEnumerable<RoomFurnitureItem> items);
    void RemoveItems(List<long> itemIds);
}