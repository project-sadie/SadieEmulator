using Sadie.Game.Navigator.Tabs;

namespace Sadie.Game.Catalog;

public class CatalogPageRepository
{
    private readonly CatalogPageDao _pageDao;
    private List<CatalogPage> _pages; // TODO: make dictionary so we can lookup by id faster

    public CatalogPageRepository(CatalogPageDao pageDao)
    {
        _pageDao = pageDao;
        _pages = new List<CatalogPage>();
    }

    public async Task LoadInitialDataAsync()
    {
        _pages = await _pageDao.GetAllAsync();
    }

    public List<CatalogPage> GetAll() => _pages;

    public Tuple<bool, CatalogPage?> TryGet(int pageId)
    {
        var page = _pages.FirstOrDefault(x => x.Id == pageId);
        return new Tuple<bool, CatalogPage?>(page != default, page);
    }
}