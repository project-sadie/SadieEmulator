using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Catalog;
using Sadie.Game.Navigator.Tabs;

namespace Sadie.Game.Navigator;

public class CatalogServiceProvider
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<CatalogPageDao>();
        serviceCollection.AddSingleton<CatalogPageFactory>();
        serviceCollection.AddSingleton<CatalogPageRepository>();
    }
}