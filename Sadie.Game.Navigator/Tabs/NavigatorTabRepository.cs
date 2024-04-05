using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Navigator;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabRepository(SadieContext dbContext)
{
    private Dictionary<string, NavigatorTabDto> _tabs = new();

    public async Task LoadInitialDataAsync()
    {
        _tabs = await dbContext.Set<NavigatorTabDto>()
            .Include(x => x.Categories)
            .ToDictionaryAsync(x => x.Name, x => x);
    }

    public bool TryGetByCodeName(string codeName, out NavigatorTabDto? tab) => 
        _tabs.TryGetValue(codeName, out tab);
}