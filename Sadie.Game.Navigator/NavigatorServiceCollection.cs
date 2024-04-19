using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Navigator.Categories;
using Sadie.Game.Navigator.Tabs;

namespace Sadie.Game.Navigator;

public static class NavigatorServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<NavigatorTabRepository>();
        serviceCollection.AddSingleton<NavigatorCategoryRepository>();
        serviceCollection.AddSingleton<NavigatorRoomProvider>();
    }
}