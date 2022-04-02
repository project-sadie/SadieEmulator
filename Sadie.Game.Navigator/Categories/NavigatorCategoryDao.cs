using Sadie.Database;

namespace Sadie.Game.Navigator.Categories;

public class NavigatorCategoryDao : BaseDao
{
    private readonly NavigatorCategoryFactory _categoryFactory;

    public NavigatorCategoryDao(IDatabaseProvider databaseProvider, NavigatorCategoryFactory categoryFactory) : base(databaseProvider)
    {
        _categoryFactory = categoryFactory;
    }

    public async Task<List<NavigatorCategory>> GetAllForTabAsync(int tabId)
    {
        var categories = new List<NavigatorCategory>();
        
        var reader = await GetReaderAsync("SELECT `id`,`name`,`code_name`,`order_id` FROM `navigator_categories` WHERE `tab_id` = @tabId;", new Dictionary<string, object>()
        {
            { "tabId", tabId }
        });

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var category = _categoryFactory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<string>("code_name"),
                record.Get<int>("order_id"));

            categories.Add(category);
        }

        return categories;
    }
}