using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;

namespace Sadie.Game.Catalog;

public static class CatalogServiceProvider
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<CatalogPage>();
        serviceCollection.AddTransient<CatalogItem>();
        serviceCollection.AddTransient<CatalogFrontPageItem>();
        serviceCollection.AddTransient<CatalogClubOffer>();
    }
}