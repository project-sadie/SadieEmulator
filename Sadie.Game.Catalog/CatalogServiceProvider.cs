using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Game.Catalog.FrontPage;
using Sadie.Game.Catalog.Pages;

namespace Sadie.Game.Catalog;

public class CatalogServiceProvider
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<CatalogPageDto>();
        serviceCollection.AddSingleton<CatalogPageRepository>();
        
        serviceCollection.AddTransient<CatalogItemDto>();
        
        serviceCollection.AddTransient<CatalogFrontPageItemDto>();
        serviceCollection.AddSingleton<CatalogFrontPageItemRepository>();
    }
}