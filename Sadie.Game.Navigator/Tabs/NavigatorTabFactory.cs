using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Navigator.Categories;

namespace Sadie.Game.Navigator.Tabs;

public class NavigatorTabFactory(IServiceProvider serviceProvider)
{
    public NavigatorTab Create(
        int id,
        string name, 
        List<NavigatorCategory> categories)
    {
        return ActivatorUtilities.CreateInstance<NavigatorTab>(
            serviceProvider,
            id, 
            name,
            categories);
    }
}
