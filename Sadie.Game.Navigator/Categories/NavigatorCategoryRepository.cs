using Microsoft.EntityFrameworkCore;
using Sadie.Database.Data;
using Sadie.Database.Models.Navigator;

namespace Sadie.Game.Navigator.Categories;

public class NavigatorCategoryRepository(SadieContext dbContext)
{
    public async Task<List<NavigatorCategoryEntity>> GetByTabIdAsync(int tabId)
    {
        return await dbContext
            .NavigatorCategories
            .Where(x => x.TabId == tabId)
            .ToListAsync();
    }
}



