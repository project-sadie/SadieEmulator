using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Navigator.Categories;
using Sadie.Game.Navigator.Tabs;

namespace Sadie.Game.Navigator;

public class NavigatorServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<NavigatorCategoryDao>();
        serviceCollection.AddSingleton<NavigatorCategoryFactory>();
        serviceCollection.AddSingleton<NavigatorCategory>();
        serviceCollection.AddSingleton<NavigatorTab>();
        serviceCollection.AddSingleton<NavigatorTabDao>();
        serviceCollection.AddSingleton<NavigatorTabRepository>();
        serviceCollection.AddSingleton<NavigatorTabFactory>();
        serviceCollection.AddSingleton<NavigatorRoomProvider>();
    }
}