namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabRepository(NavigatorTabDao tabDao)
{
    private Dictionary<string, NavigatorTab> _tabs = new();

    public async Task LoadInitialDataAsync()
    {
        _tabs = (await tabDao.GetAllAsync())
            .ToDictionary(x => x.Name, x => x);
    }

    public bool TryGetByCodeName(string codeName, out NavigatorTab? tab) => 
        _tabs.TryGetValue(codeName, out tab);
}