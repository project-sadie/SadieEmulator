namespace Sadie.Game.Players.Inventory;

public interface IPlayerInventoryRepository
{
    ICollection<PlayerInventoryFurnitureItem> Items { get; }
    void AddItems(IEnumerable<PlayerInventoryFurnitureItem> items);
    void RemoveItems(List<long> itemIds);
}