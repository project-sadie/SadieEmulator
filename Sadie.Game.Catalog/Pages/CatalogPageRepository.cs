using Sadie.Game.Navigator.Tabs;

namespace Sadie.Game.Catalog;

public class CatalogPageRepository
{
    private readonly CatalogPageDao _pageDao;
    private Dictionary<int, CatalogPage> _pages; // TODO: make dictionary so we can lookup by id faster

    public CatalogPageRepository(CatalogPageDao pageDao)
    {
        _pageDao = pageDao;
        _pages = new Dictionary<int, CatalogPage>();
    }

    public async Task LoadInitialDataAsync()
    {
        _pages = await _pageDao.GetAllAsync();
    }

    public Dictionary<int, CatalogPage>.ValueCollection GetAll() => _pages.Values;

    public Tuple<bool, CatalogPage?> TryGet(int pageId)
    {
        return new Tuple<bool, CatalogPage?>(_pages.ContainsKey(pageId), _pages[pageId]);
    }
}