using Sadie.Database;

namespace Sadie.Game.Navigator.Categories;

public class NavigatorCategoryDao(IDatabaseProvider databaseProvider, NavigatorCategoryFactory categoryFactory)
    : BaseDao(databaseProvider)
{
    public async Task<List<NavigatorCategory>> GetAllForTabAsync(int tabId)
    {
        var categories = new List<NavigatorCategory>();
        
        var reader = await GetReaderAsync(@"
            SELECT id, name, code_name, order_id 
            FROM navigator_categories 
            WHERE tab_id = @tabId;", new Dictionary<string, object>
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

            var category = categoryFactory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"),
                record.Get<string>("code_name"),
                record.Get<int>("order_id"));

            categories.Add(category);
        }

        return categories;
    }
}