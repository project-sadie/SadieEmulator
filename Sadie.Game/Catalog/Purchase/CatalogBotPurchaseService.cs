using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Catalog.Items;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Game.Catalog;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Core.Enums.Game.Players;
using Sadie.Db;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Game.Catalog.Purchase;

public class CatalogBotPurchaseService(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    ICatalogPurchaseConfirmationService confirmationService) : ICatalogBotPurchaseService
{
    public async Task ProcessAsync(INetworkClient client, CatalogItemDto item)
    {
        var data = item.MetaData;

        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        var info = data.Split(";")
            .ToDictionary(x => x.Split(":")[0], x => x.Split(":")[1]);

        var bot = new PlayerBotDto
        {
            PlayerId = client.Player.Player.Id,
            Username = info["name"],
            FigureCode = info["figure"],
            Motto = info["motto"],
            Gender = info["gender"].ToUpper() == "M"
                ? PlayerAvatarGender.Male
                : PlayerAvatarGender.Female,
            CreatedAt = DateTime.Now
        };

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(bot).State = EntityState.Added;
        await dbContext.SaveChangesAsync();

        client.Player.Player.Bots.Add(bot);

        await client.WriteToStreamAsync(new PlayerInventoryAddBotWriter
        {
            Id = bot.Id,
            Username = bot.Username,
            Motto = bot.Motto,
            Gender = bot.Gender == PlayerAvatarGender.Male ? "m" : "f",
            FigureCode = bot.FigureCode,
            OpenInventory = true
        });

        await confirmationService.ConfirmAsync(client, item, 1);
    }
}