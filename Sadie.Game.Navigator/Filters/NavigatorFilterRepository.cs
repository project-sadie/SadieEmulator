namespace Sadie.Game.Navigator.Filters;

public class NavigatorFilterRepository(NavigatorFilterDao filterDao)
{
    private Dictionary<string, NavigatorFilter> _filters = new();

    public async Task LoadInitialDataAsync()
    {
        _filters = (await filterDao.GetAllAsync())
            .ToDictionary(x => x.Name, x => x);
    }

    public bool TryGet(string name, out NavigatorFilter? filter) => 
        _filters.TryGetValue(name, out filter);
}