using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Game.Navigator;
using Sadie.Game.Navigator.Categories;

namespace Sadie.Game.Navigator;

public static class NavigatorServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<NavigatorCategoryRepository>();
        serviceCollection.AddTransient<INavigatorRoomProvider, NavigatorRoomProvider>();
    }
}