using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Navigator.Categories;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabFactory
{
    private readonly IServiceProvider _serviceProvider;

    public NavigatorTabFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public NavigatorTab Create(
        int id,
        string name, 
        List<NavigatorCategory> categories)
    {
        return ActivatorUtilities.CreateInstance<NavigatorTab>(
            _serviceProvider,
            id, 
            name,
            categories);
    }
}
