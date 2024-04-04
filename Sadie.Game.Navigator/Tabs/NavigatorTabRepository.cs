using Microsoft.EntityFrameworkCore;
using Sadie.Database.Data;
using Sadie.Database.Models.Navigator;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabRepository(SadieContext dbContext)
{
    private Dictionary<string, NavigatorTabEntity> _tabs = new();

    public async Task LoadInitialDataAsync()
    {
        _tabs = await dbContext.Set<NavigatorTabEntity>()
            .Include(x => x.Categories)
            .ToDictionaryAsync(x => x.Name, x => x);
    }

    public bool TryGetByCodeName(string codeName, out NavigatorTabEntity? tab) => 
        _tabs.TryGetValue(codeName, out tab);
}