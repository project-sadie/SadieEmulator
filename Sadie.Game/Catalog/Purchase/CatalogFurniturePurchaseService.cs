using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Catalog.Items;
using Sadie.API.DTOs.Furniture;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Db;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Networking.Writers.Players;

namespace Sadie.Game.Catalog.Purchase;

public class CatalogFurniturePurchaseService(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    ICatalogPurchaseConfirmationService confirmationService,
    IMapper mapper) : ICatalogFurniturePurchaseService
{
    public async Task ProcessAsync(INetworkClient client, CatalogItemDto item, string? metaData, int amount)
    {
        var created = DateTime.Now;
        var furniture = mapper.Map<FurnitureItemDto>(item.FurnitureItems.First());
        var newItems = new List<PlayerFurnitureItemDto>();

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        for (var i = 0; i < amount; i++)
        {
            var dto = new PlayerFurnitureItemDto
            {
                PlayerId = client.Player.Player.Id,
                FurnitureItemId = furniture.Id,
                FurnitureItem = furniture,
                LimitedData = "1:1",
                MetaData = metaData ?? "",
                CreatedAt = created
            };

            client.Player.Player.FurnitureItems.Add(dto);
            dbContext.PlayerFurnitureItems.Add(mapper.Map<PlayerFurnitureItem>(dto));
            newItems.Add(dto);
        }

        await dbContext.SaveChangesAsync();

        await client.WriteToStreamAsync(new PlayerInventoryUnseenItemsWriter
        {
            Count = newItems.Count,
            Category = 1,
            FurnitureItems = newItems
        });

        await confirmationService.ConfirmAsync(client, item, amount);
    }
}