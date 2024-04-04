using Sadie.Database.LegacyAdoNet;
using Sadie.Game.Furniture;

namespace Sadie.Game.Catalog.Items;

public class CatalogItemDao(
    IDatabaseProvider databaseProvider,
    CatalogItemFactory factory,
    FurnitureItemRepository furnitureItemRepository)
    : BaseDao(databaseProvider)
{
    public async Task<List<CatalogItem>> GetAllAsync()
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
                furniture_item_ids,
                meta_data,
                catalog_page_id,
                amount,
                sell_limit
            FROM catalog_items;");

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var definitions = record.Get<string>("furniture_item_ids").Split(";");
            var furnitureItemIds = definitions.Select(x => int.Parse(x.Split(":")[0]));
            var furnitureItems = new List<FurnitureItem>();

            foreach (var itemId in furnitureItemIds)
            {
                var result = furnitureItemRepository.TryGetById(itemId);

                if (result is { Item1: true, Item2: not null })
                {
                    furnitureItems.Add(result.Item2);
                }
            }

            if (!furnitureItems.Any())
            {
                continue;
            }

            var item = factory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<int>("cost_credits"),
                record.Get<int>("cost_points"),
                record.Get<int>("cost_points_type"),
                furnitureItems,
                record.Get<bool>("requires_club_membership"),
                record.Get<string>("meta_data"),
                record.Get<long>("catalog_page_id"),
                record.Get<int>("amount"),
                record.Get<int>("sell_limit"));
                
            items.Add(item);
        }

        return items;
    }
}