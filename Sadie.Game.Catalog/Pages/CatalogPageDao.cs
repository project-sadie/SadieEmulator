using Sadie.Database;
using Sadie.Game.Catalog;

namespace Sadie.Game.Navigator.Tabs;

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
                childPages, 
                items);

            pages.Add(page);
        }

        return pages;
    }

    public async Task<List<CatalogPage>> GetAllAsync()
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
                childPages, 
                items);

            pages.Add(page);
        }

        return pages;
    }
}