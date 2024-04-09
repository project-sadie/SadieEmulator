using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.Pages;

namespace Sadie.Game.Catalog.Pages;

public class CatalogPageRepository(SadieContext dbContext)
{
    private Dictionary<int, CatalogPage> _pages = [];

    public async Task LoadInitialDataAsync()
    {
        _pages = await dbContext.Set<CatalogPage>()
            .Include(x => x.Pages)
            .Include(x => x.Items)
            .ThenInclude(x => x.FurnitureItems)
            .ToDictionaryAsync(x => x.Id, x => x);
    }

    public CatalogPage? TryGet(int pageId)
    {
        return _pages.GetValueOrDefault(pageId);
    }

    public List<CatalogPage> GetByParentId(int parentId)
    {
        return _pages
            .Values
            .Where(x => x.CatalogPageId == parentId)
            .ToList();
    }

    public CatalogPage? TryGetByLayout(string layout)
    {
        return _pages.Values.FirstOrDefault(x => x.Layout == layout);
    }
}