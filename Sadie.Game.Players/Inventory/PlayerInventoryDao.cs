using Sadie.Database;
using Sadie.Game.Furniture;

namespace Sadie.Game.Players.Inventory;

public class PlayerInventoryDao(
    IDatabaseProvider databaseProvider, 
    FurnitureItemRepository furnitureRepository) : BaseDao(databaseProvider), IPlayerInventoryDao
{
    public async Task<List<PlayerInventoryFurnitureItem>> GetAllAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                id, 
                player_id, 
                furniture_item_id, limited_data, meta_data, created_at FROM player_furniture_items
            WHERE player_id = @playerId", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerInventoryFurnitureItem>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(CreateItemFromRecord(record));
        }
        
        return data;
    }

    private PlayerInventoryFurnitureItem CreateItemFromRecord(DatabaseRecord record)
    {
        var (found, furnitureItem) = furnitureRepository.TryGetById(record.Get<int>("furniture_item_id"));

        if (!found || furnitureItem == null)
        {
            throw new Exception("Unable to fetch furniture item for a players inventory item.");
        }

        return new PlayerInventoryFurnitureItem(
            record.Get<long>("id"),
            furnitureItem,
            record.Get<string>("limited_data"),
            record.Get<string>("meta_data"),
            record.Get<DateTime>("created_at"));
    }

    public async Task<int> CreateItemAsync(long playerId, PlayerInventoryFurnitureItem item)
    {
        var parameters = new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "furnitureId", item.Item.Id },
            { "metaData", item.MetaData },
            { "createdAt", item.Created }
        };

        return await QueryScalarAsync(@"
            INSERT INTO player_furniture_items (
                player_id, furniture_item_id, meta_data, created_at
            ) VALUES (@playerId, @furnitureId, @metaData, @createdAt); SELECT LAST_INSERT_ID();", parameters);
    }

    public async Task DeleteItemsAsync(List<long> itemIds)
    {
        await QueryAsync($"DELETE FROM player_furniture_items WHERE id IN ({string.Join(",", itemIds)})");
    }
}