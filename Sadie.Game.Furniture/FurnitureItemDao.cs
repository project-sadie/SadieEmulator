using Sadie.Database;

namespace Sadie.Game.Furniture;

public class FurnitureItemDao : BaseDao
{
    private readonly FurnitureItemFactory _factory;

    public FurnitureItemDao(IDatabaseProvider databaseProvider, FurnitureItemFactory factory) : base(databaseProvider)
    {
        _factory = factory;
    }
    
    public async Task<Dictionary<int, FurnitureItem>> GetAllAsync()
    {
        var furnitureItems = new Dictionary<int, FurnitureItem>();
        
        var reader = await GetReaderAsync(@"
            SELECT 
                id,
                `name`,
                asset_name,
                type,
                asset_id,
                tile_span_x,
                tile_span_y,
                stack_height,
                can_stack,
                can_walk,
                can_sit,
                can_lay,
                can_recycle,
                can_trade,
                can_marketplace_sell,
                can_inventory_stack,
                can_gift,
                interaction_type,
                interaction_modes
            FROM furniture_items;");

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var item = _factory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<string>("asset_name"),
                record.Get<char>("type"),
                record.Get<int>("asset_id"),
                record.Get<int>("tile_span_x"),
                record.Get<int>("tile_span_y"),
                record.Get<double>("stack_height"),
                record.Get<bool>("can_stack"),
                record.Get<bool>("can_walk"),
                record.Get<bool>("can_sit"),
                record.Get<bool>("can_lay"),
                record.Get<bool>("can_recycle"),
                record.Get<bool>("can_trade"),
                record.Get<bool>("can_marketplace_sell"),
                record.Get<bool>("can_inventory_stack"),
                record.Get<bool>("can_gift"),
                record.Get<string>("interaction_type"),
                record.Get<int>("interaction_modes"));
            
            furnitureItems[item.Id] = item;
        }

        return furnitureItems;
    }
}