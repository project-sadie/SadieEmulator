using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Navigator.Categories;
using Sadie.Game.Navigator.Filters;
using Sadie.Game.Navigator.Tabs;

namespace Sadie.Game.Navigator;

public static class NavigatorServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<NavigatorFilterDao>();
        serviceCollection.AddSingleton<NavigatorFilterFactory>();
        serviceCollection.AddSingleton<NavigatorFilterRepository>();
        serviceCollection.AddTransient<NavigatorFilter>();
        
        serviceCollection.AddSingleton<NavigatorCategoryDao>();
        serviceCollection.AddSingleton<NavigatorCategoryFactory>();
        serviceCollection.AddTransient<NavigatorCategory>();
        
        serviceCollection.AddTransient<NavigatorTab>();
        serviceCollection.AddSingleton<NavigatorTabDao>();
        serviceCollection.AddSingleton<NavigatorTabRepository>();
        serviceCollection.AddSingleton<NavigatorTabFactory>();
        
        serviceCollection.AddSingleton<NavigatorRoomProvider>();
    }
}