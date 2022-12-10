using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Furniture;

namespace Sadie.Game.Catalog;

public class CatalogItemDao : BaseDao
{
    private readonly ILogger<CatalogItemDao> _logger;
    private readonly CatalogItemFactory _factory;
    private readonly FurnitureItemRepository _furnitureItemRepository;

    public CatalogItemDao(
        ILogger<CatalogItemDao> logger,
        IDatabaseProvider databaseProvider, 
        CatalogItemFactory factory,
        FurnitureItemRepository furnitureItemRepository) : base(databaseProvider)
    {
        _logger = logger;
        _factory = factory;
        _furnitureItemRepository = furnitureItemRepository;
    }

    public async Task<List<CatalogItem>> GetItemsForPageAsync(int pageId)
    {
        var items = new List<CatalogItem>();
        
        var reader = await GetReaderAsync(@"
            SELECT 
                id,
                name,
                cost_credits,
                cost_points,
                cost_points_type,
                requires_club_membership,
                furniture_item_id,
                meta_data,
                catalog_page_id,
                amount,
                sell_limit
            FROM catalog_items
            WHERE catalog_page_id = @pageId;", new Dictionary<string, object>
        {
            { "pageId", pageId }
        });

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var (found, furnitureItem) = _furnitureItemRepository.TryGetById(record.Get<int>("furniture_item_id"));

            if (!found)
            {
                continue;
            }

            var item = _factory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<int>("cost_credits"),
                record.Get<int>("cost_points"),
                record.Get<int>("cost_points_type"),
                furnitureItem,
                record.Get<bool>("requires_club_membership"),
                record.Get<string>("meta_data"),
                record.Get<int>("amount"),
                record.Get<int>("sell_limit"));
                
            items.Add(item);
        }

        return items;
    }
}