using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Navigator.Filters;

public class NavigatorFilterFactory(IServiceProvider serviceProvider)
{
    public NavigatorFilter Create(string name)
    {
        return ActivatorUtilities.CreateInstance<NavigatorFilter>(
            serviceProvider, name);
    }
}