using Sadie.Database;
using Sadie.Game.Navigator.Categories;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabDao : BaseDao
{
    private readonly NavigatorTabFactory _tabFactory;
    private readonly NavigatorCategoryDao _navigatorCategoryDao;

    public NavigatorTabDao(
        IDatabaseProvider databaseProvider, 
        NavigatorTabFactory tabFactory, 
        NavigatorCategoryDao navigatorCategoryDao) : base(databaseProvider)
    {
        _tabFactory = tabFactory;
        _navigatorCategoryDao = navigatorCategoryDao;
    }

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

            var categories = await _navigatorCategoryDao.GetAllForTabAsync(record.Get<int>("id"));

            var tab = _tabFactory.Create(
                record.Get<int>("id"),
                record.Get<string>("name"), 
                categories);
            
            tabs.Add(tab);
        }

        return tabs;
    }
}