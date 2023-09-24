using Microsoft.Extensions.DependencyInjection;
using Sadie.Game.Catalog.Items;
using Sadie.Game.Catalog.Pages;

namespace Sadie.Game.Catalog;

public class CatalogServiceProvider
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<CatalogPage>();
        serviceCollection.AddSingleton<CatalogPageDao>();
        serviceCollection.AddSingleton<CatalogPageFactory>();
        serviceCollection.AddSingleton<CatalogPageRepository>();
        
        serviceCollection.AddTransient<CatalogItem>();
        serviceCollection.AddSingleton<CatalogItemDao>();
        serviceCollection.AddSingleton<CatalogItemFactory>();
    }
}