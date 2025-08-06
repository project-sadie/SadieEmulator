using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Enums.Game.Players;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerIgnoredUser)]
public class PlayerIgnoreUserEventHandler(IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory) : INetworkPacketEventHandler
{
    public required string Username { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;

        if (player?.NetworkObject == null)
        {
            return;
        }
        
        var targetPlayer = playerRepository.GetPlayerLogicByUsername(Username);
        
        if (targetPlayer == null || 
            player.Ignores.Any(x => x.TargetPlayerId == targetPlayer.Id))
        {
            return;
        }

        var ignore = new PlayerIgnore
        {
            PlayerId = player.Id,
            TargetPlayerId = targetPlayer.Id
        };

        player.Ignores.Add(ignore);

        await player.NetworkObject.WriteToStreamAsync(
            new PlayerIgnoreStateWriter
            {
                State = (int) PlayerIgnoreState.Ignored,
                Username = targetPlayer.Username,
            });
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        dbContext.Entry(ignore).State = EntityState.Added;
        await dbContext.SaveChangesAsync();
    }
}