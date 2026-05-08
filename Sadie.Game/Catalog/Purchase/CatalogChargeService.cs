using Sadie.API.DTOs.Catalog.Items;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Core.Players;
using Sadie.Networking.Writers.Players.Purse;

namespace Sadie.Game.Catalog.Purchase;

public class CatalogChargeService : ICatalogChargeService
{
    public bool HasRequiredMembership(INetworkClient client, CatalogItemDto item)
    {
        if (!item.RequiresClubMembership)
        {
            return true;
        }

        return client.Player?.Player.Subscriptions
            .FirstOrDefault(x => x.Subscription.Name == "HABBO_CLUB") != null;
    }

    public async Task<bool> TryChargeAsync(INetworkClient client, CatalogItemDto item, int amount)
    {
        var costCredits = item.CostCredits * amount;
        var costPoints = item.CostPoints * amount;
        var data = client.Player.Player.Data;

        if (data.CreditBalance < costCredits ||
            (item.CostPointsType == 0 && data.PixelBalance < costPoints) ||
            (item.CostPointsType != 0 && data.SeasonalBalance < costPoints))
        {
            return false;
        }

        if (costCredits > 0)
        {
            data.CreditBalance -= costCredits;

            await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
            {
                Credits = data.CreditBalance
            });
        }

        if (costPoints <= 0)
        {
            return true;
        }
        
        if (item.CostPointsType == 0)
        {
            data.PixelBalance -= costPoints;
        }
        else
        {
            data.SeasonalBalance -= costPoints;
        }

        await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
        {
            Currencies = PlayerCurrencyMapper.FromBalances(
                data.PixelBalance,
                data.SeasonalBalance,
                data.GotwPoints)
        });

        return true;
    }
}