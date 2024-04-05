using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Catalog.FrontPage;

namespace Sadie.Game.Catalog.FrontPage;

public class CatalogFrontPageItemRepository(SadieContext dbContext)
{
    public List<CatalogFrontPageItemDto> Items = [];

    public async Task LoadInitialDataAsync()
    {
        Items = await dbContext.Set<CatalogFrontPageItemDto>().ToListAsync();
    }
}