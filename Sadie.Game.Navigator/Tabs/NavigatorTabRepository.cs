using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Navigator;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabRepository(SadieContext dbContext)
{
    private Dictionary<string, NavigatorTab> _tabs = new();

    public async Task LoadInitialDataAsync()
    {
        _tabs = await dbContext.Set<NavigatorTab>()
            .Include(x => x.Categories)
            .ToDictionaryAsync(x => x.Name, x => x);
    }

    public bool TryGetByCodeName(string codeName, out NavigatorTab? tab) => 
        _tabs.TryGetValue(codeName, out tab);
}