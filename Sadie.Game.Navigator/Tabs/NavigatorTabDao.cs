using Sadie.Database;
using Sadie.Game.Navigator.Categories;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabDao(
    IDatabaseProvider databaseProvider,
    NavigatorTabFactory tabFactory,
    NavigatorCategoryDao navigatorCategoryDao)
    : BaseDao(databaseProvider)
{
    public async Task<List<NavigatorTab>> GetAllAsync()
    {
        var tabs = new List<NavigatorTab>();
        var reader = await GetReaderAsync("SELECT id, `name` FROM navigator_tabs;");

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var categories = await navigatorCategoryDao.GetAllForTabAsync(record.Get<int>("id"));

            var tab = tabFactory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"), 
                categories);
            
            tabs.Add(tab);
        }

        return tabs;
    }
}