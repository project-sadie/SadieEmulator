namespace Sadie.Game.Players.Inventory;

public interface IPlayerInventoryDao
{
    Task<List<PlayerInventoryFurnitureItem>> GetAllAsync(long playerId);
    Task<int> InsertItemAsync(int playerId, PlayerInventoryFurnitureItem items);
}