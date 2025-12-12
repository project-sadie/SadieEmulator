using AutoMapper;
using Sadie.API.DTOs.Catalog.Items;
using Sadie.API.DTOs.Furniture;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Core.Enums.Game.Catalog;
using Sadie.Networking.Writers.Catalog;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Game.Catalog.Purchase;

public class CatalogPurchaseConfirmationService(IMapper mapper) : ICatalogPurchaseConfirmationService
{
    public async Task ConfirmAsync(INetworkClient client, CatalogItemDto item, int amount)
    {
        await client.WriteToStreamAsync(new CatalogPurchaseOkWriter
        {
            Id = item.Id,
            Name = item.Name,
            Rented = false,
            CostCredits = item.CostCredits,
            CostPoints = item.CostPoints,
            CostPointsType = item.CostPointsType,
            CanGift = item.FurnitureItems.First().CanGift,
            FurnitureItems = mapper.Map<List<FurnitureItemDto>>(item.FurnitureItems),
            Amount = amount,
            ClubLevel = item.RequiresClubMembership ? 1 : 0,
            CanPurchaseBundles = item.Amount != 1,
            Metadata = item.MetaData,
            IsLimited = false,
            LimitedItemSeriesSize = 0,
            AmountLeft = 0
        });

        await client.WriteToStreamAsync(new PlayerInventoryRefreshWriter());
    }

    public async Task WriteFailureAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogPurchaseFailedWriter
        {
            Error = (int)CatalogPurchaseError.Server
        });
    }

    public async Task WriteUnavailableAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new CatalogPurchaseUnavailableWriter
        {
            Code = 1
        });
    }
}