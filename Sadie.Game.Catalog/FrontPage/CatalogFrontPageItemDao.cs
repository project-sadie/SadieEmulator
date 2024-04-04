using Sadie.Database.LegacyAdoNet;
using Sadie.Game.Catalog.Pages;

namespace Sadie.Game.Catalog.FrontPage;

public class CatalogFrontPageItemDao(
    IDatabaseProvider databaseProvider, 
    CatalogFrontPageItemFactory factory,
    CatalogPageRepository pageRepository) : BaseDao(databaseProvider)
{
    public async Task<List<CatalogFrontPageItem>> GetAllAsync()
    {
        var items = new List<CatalogFrontPageItem>();
        var reader = await GetReaderAsync("SELECT id, title, image, type_id, product_name, catalog_page_id FROM catalog_front_page_items;");

        while (true)        
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            CatalogPage? matchingPage = null;

            if (record.Get<object>("catalog_page_id") != DBNull.Value)
            {
                var (found, page) = pageRepository.TryGet(record.Get<int>("catalog_page_id"));

                if (!found || page == null)
                {
                    continue;
                }

                matchingPage = page;
            }

            var filter = factory.Create(
                record.Get<int>("id"),
                record.Get<string>("title"),
                record.Get<string>("image"),
                (CatalogFrontPageItemType) record.Get<int>("type_id"),
                record.Get<string>("product_name"),
                matchingPage!);
            
            items.Add(filter);
        }

        return items;
    }
}