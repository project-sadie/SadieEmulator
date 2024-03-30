namespace Sadie.Game.Players.Inventory;

public interface IPlayerInventoryDao
{
    Task<List<PlayerInventoryFurnitureItem>> GetAllAsync(long playerId);
    Task<int> CreateItemAsync(long playerId, PlayerInventoryFurnitureItem items);
    Task DeleteItemsAsync(List<long> itemIds);
}