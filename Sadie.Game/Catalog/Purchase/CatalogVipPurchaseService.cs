using Microsoft.EntityFrameworkCore;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Core.Enums.Game.Catalog;
using Sadie.Db;
using Sadie.Db.Models.Catalog.Items;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Game.Catalog.Purchase;

public class CatalogVipPurchaseService(
    IDbContextFactory<SadieDbContext> dbContextFactory) : ICatalogVipPurchaseService
{
    public async Task ProcessAsync(INetworkClient client, int itemId)
    {
        if (client.Player == null)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        var item = await dbContext
            .Set<CatalogItem>()
            .FirstOrDefaultAsync(x => x.Id == itemId);

        if (item == null)
        {
            await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
            {
                Error = (int)CatalogPurchaseError.Server
            });

            return;
        }

        await client.WriteToStreamAsync(new CatalogPurchaseOkWriter
        {
            Id = item.Id,
            Name = item.Name,
            Rented = false,
            CostCredits = item.CostCredits,
            CostPoints = item.CostPoints,
            CostPointsType = item.CostPointsType,
            CanGift = false,
            FurnitureItems = [],
            Amount = 1,
            ClubLevel = 1,
            CanPurchaseBundles = false,
            Metadata = item.MetaData,
            IsLimited = false,
            LimitedItemSeriesSize = 0,
            AmountLeft = 0
        });

        await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
    }
}