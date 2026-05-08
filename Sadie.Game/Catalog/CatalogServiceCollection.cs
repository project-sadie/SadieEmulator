using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.Game.Catalog.Purchase;

namespace Sadie.Game.Catalog;

public class CatalogServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddSingleton<ICatalogPageRepository, CatalogPagePageRepository>();
        serviceCollection.AddSingleton<ICatalogChargeService, CatalogChargeService>();
        serviceCollection.AddSingleton<ICatalogFurniturePurchaseService, CatalogFurniturePurchaseService>();
        serviceCollection.AddSingleton<ICatalogBotPurchaseService, CatalogBotPurchaseService>();
        serviceCollection.AddSingleton<ICatalogPurchaseConfirmationService, CatalogPurchaseConfirmationService>();
        serviceCollection.AddSingleton<ICatalogVipPurchaseService, CatalogVipPurchaseService>();
    }
}