using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Pages;

namespace Sadie.Game.Catalog.Pages;

public class CatalogPageRepository(SadieContext dbContext)
{
    private Dictionary<int, CatalogPageDto> _pages = new();

    public async Task LoadInitialDataAsync()
    {
        _pages = await dbContext.Set<CatalogPageDto>()
            .ToDictionaryAsync(x => x.Id, x => x);
    }

    public Tuple<bool, CatalogPageDto?> TryGet(int pageId)
    {
        return new Tuple<bool, CatalogPageDto?>(_pages.ContainsKey(pageId), _pages[pageId]);
    }

    public List<CatalogPageDto> GetByParentId(int parentId)
    {
        return _pages
            .Values
            .Where(x => x.ParentId == parentId)
            .ToList();
    }

    public Tuple<bool, CatalogPageDto?> TryGetByLayout(string layout)
    {
        var page = _pages.Values.FirstOrDefault(x => x.Layout == layout);
        return new Tuple<bool, CatalogPageDto?>(page != null, page);
    }
}