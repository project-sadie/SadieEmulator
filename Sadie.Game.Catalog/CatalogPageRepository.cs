using Sadie.Game.Navigator.Tabs;

namespace Sadie.Game.Catalog;

public class CatalogPageRepository
{
    private readonly CatalogPageDao _pageDao;
    private List<CatalogPage> _pages;

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
}