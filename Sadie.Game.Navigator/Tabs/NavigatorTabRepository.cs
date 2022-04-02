namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabRepository
{
    private readonly NavigatorTabDao _tabDao;
    private Dictionary<string, NavigatorTab> _tabs;

    public NavigatorTabRepository(NavigatorTabDao tabDao)
    {
        _tabDao = tabDao;
        _tabs = new Dictionary<string, NavigatorTab>();
    }

    public async Task LoadInitialDataAsync()
    {
        _tabs = (await _tabDao.GetAllAsync())
            .ToDictionary(x => x.Name, x => x);
    }

    public bool TryGetByCodeName(string codeName, out NavigatorTab? tab) => 
        _tabs.TryGetValue(codeName, out tab);
}