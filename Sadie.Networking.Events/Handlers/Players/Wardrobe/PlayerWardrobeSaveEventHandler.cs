using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Players;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

[PacketId(EventHandlerId.PlayerWardrobeSave)]
public class PlayerWardrobeSaveEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : INetworkPacketEventHandler
{
    public int SlotId { get; set; }
    public required string FigureCode { get; set; }
    public required string Gender { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;

        if (player == null)
        {
            return;
        }

        var wardrobeItem = new PlayerWardrobeItem
        {
            SlotId = SlotId,
            FigureCode = FigureCode,
            Gender = Gender == "M" ? PlayerAvatarGender.Male : PlayerAvatarGender.Female
        };
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerWardrobeItems.Add(wardrobeItem);
        dbContext.Entry(wardrobeItem).State = EntityState.Added;
        await dbContext.SaveChangesAsync();
            
        player.WardrobeItems.Add(
            mapper.Map<PlayerWardrobeItemDto>(wardrobeItem));
    }
}