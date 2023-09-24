using Sadie.Database;
using Sadie.Game.Catalog.Items;

namespace Sadie.Game.Catalog.Pages;

public class CatalogPageDao : BaseDao
{
    private readonly CatalogItemDao _itemDao;
    private readonly CatalogPageFactory _factory;

    public CatalogPageDao(IDatabaseProvider databaseProvider, CatalogItemDao itemDao, CatalogPageFactory factory) : base(databaseProvider)
    {
        _itemDao = itemDao;
        _factory = factory;
    }

    private async Task<List<CatalogPage>> GetAllChildPagesAsync(int parentId)
    {
        var pages = new List<CatalogPage>();
        
        var reader = await GetReaderAsync(@"
            SELECT 
                id, 
                `name`, 
                caption, 
                layout, 
                role_id, 
                parent_page_id, 
                order_id, 
                icon_id, 
                enabled, 
                visible 
            FROM catalog_pages 
            WHERE parent_page_id = @parentPageId;", new Dictionary<string, object>
        {
            { "parentPageId", parentId }
        });

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var childPages = new List<CatalogPage>(); // await GetAllChildPagesAsync(record.Get<int>("id"));
            var items = await _itemDao.GetItemsForPageAsync(record.Get<int>("id"));

            var page = _factory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<string>("caption"),
                record.Get<string>("layout"),
                record.Get<int>("role_id"),
                record.Get<int>("parent_page_id"),
                record.Get<int>("order_id"),
                record.Get<int>("icon_id"),
                record.Get<int>("enabled") == 1,
                record.Get<int>("visible") == 1, 
                record.Get<string>("header_image"),
                record.Get<string>("teaser_image"),
                record.Get<string>("special_image"),
                record.Get<string>("primary_text"),
                record.Get<string>("secondary_text"),
                record.Get<string>("details_text"),
                record.Get<string>("teaser_text"),
                childPages,
                items);

            pages.Add(page);
        }

        return pages;
    }

    public async Task<Dictionary<int, CatalogPage>> GetAllAsync()
    {
        var pages = new Dictionary<int, CatalogPage>();
        
        var reader = await GetReaderAsync(@"
            SELECT 
                id, 
                `name`, 
                caption, 
                layout, 
                role_id, 
                parent_page_id, 
                order_id, 
                icon_id, 
                enabled, 
                visible 
            FROM catalog_pages;");

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var childPages = await GetAllChildPagesAsync(record.Get<int>("id"));
            var items = await _itemDao.GetItemsForPageAsync(record.Get<int>("id"));

            var page = _factory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<string>("caption"),
                record.Get<string>("layout"),
                record.Get<int>("role_id"),
                record.Get<int>("parent_page_id"),
                record.Get<int>("order_id"),
                record.Get<int>("icon_id"),
                record.Get<int>("enabled") == 1,
                record.Get<int>("visible") == 1,
                record.Get<string>("header_image"),
                record.Get<string>("teaser_image"),
                record.Get<string>("special_image"),
                record.Get<string>("primary_text"),
                record.Get<string>("secondary_text"),
                record.Get<string>("details_text"),
                record.Get<string>("teaser_text"),
                childPages, 
                items);

            pages.Add(record.Get<int>("id"), page);
        }

        return pages;
    }
}