namespace Sadie.Game.Catalog.Pages;

public class CatalogPageRepository(CatalogPageDao pageDao)
{
    private Dictionary<int, CatalogPage> _pages = new(); // TODO: make dictionary so we can lookup by id faster

    public async Task LoadInitialDataAsync()
    {
        _pages = await pageDao.GetAllAsync();

        foreach (var page in _pages.Values)
        {
            if (_pages.TryGetValue(page.ParentId, out var parentPage) && page.Id != parentPage.Id)
            {
                parentPage.Pages.Add(page);
            }
        }
    }

    public Tuple<bool, CatalogPage?> TryGet(int pageId)
    {
        return new Tuple<bool, CatalogPage?>(_pages.ContainsKey(pageId), _pages[pageId]);
    }

    public List<CatalogPage> GetByParentId(int parentId)
    {
        return _pages
            .Values
            .Where(x => x.ParentId == parentId)
            .ToList();
    }
}