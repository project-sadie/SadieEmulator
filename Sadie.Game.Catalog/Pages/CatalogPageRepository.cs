using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Pages;

namespace Sadie.Game.Catalog.Pages;

public class CatalogPageRepository(SadieContext dbContext)
{
    private Dictionary<int, CatalogPage> _pages = new();

    public async Task LoadInitialDataAsync()
    {
        _pages = await dbContext.Set<CatalogPage>()
            .ToDictionaryAsync(x => x.Id, x => x);
    }

    public Tuple<bool, CatalogPage?> TryGet(int pageId)
    {
        return new Tuple<bool, CatalogPage?>(_pages.ContainsKey(pageId), _pages[pageId]);
    }

    public List<CatalogPage> GetByParentId(int parentId)
    {
        return _pages
            .Values
            .Where(x => x.CatalogPageId == parentId)
            .ToList();
    }

    public Tuple<bool, CatalogPage?> TryGetByLayout(string layout)
    {
        var page = _pages.Values.FirstOrDefault(x => x.Layout == layout);
        return new Tuple<bool, CatalogPage?>(page != null, page);
    }
}