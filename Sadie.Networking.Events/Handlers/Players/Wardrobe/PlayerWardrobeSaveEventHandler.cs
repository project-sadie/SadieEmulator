using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Wardrobe;

[PacketId(EventHandlerId.PlayerWardrobeSave)]
public class PlayerWardrobeSaveEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
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
            Gender = Gender == "M" ? AvatarGender.Male : AvatarGender.Female
        };
            
        player.WardrobeItems.Add(wardrobeItem);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(wardrobeItem).State = EntityState.Added;
        await dbContext.SaveChangesAsync();
    }
}