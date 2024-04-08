using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Catalog;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Game.Catalog.Club;
using Sadie.Game.Catalog.FrontPage;
using Sadie.Game.Catalog.Pages;

namespace Sadie.Game.Catalog;

public static class CatalogServiceProvider
{
    public static void AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<CatalogPage>();
        serviceCollection.AddSingleton<CatalogPageRepository>();
        
        serviceCollection.AddTransient<CatalogItem>();
        
        serviceCollection.AddTransient<CatalogFrontPageItem>();
        serviceCollection.AddSingleton<CatalogFrontPageItemRepository>();
        
        serviceCollection.AddSingleton<CatalogClubOfferRepository>();
        serviceCollection.AddTransient<CatalogClubOffer>();
    }
}