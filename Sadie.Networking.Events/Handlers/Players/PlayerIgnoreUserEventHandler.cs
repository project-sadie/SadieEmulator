using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
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

        var ignore = new PlayerIgnoreDto
        {
            PlayerId = player.Id,
            TargetPlayerId = targetPlayer.Id
        };

        player.Ignores.Add(ignore);

        await player.NetworkObject.WriteToStreamAsync(
            new PlayerIgnoreStateWriter
            {
                State = (int) PlayerIgnoreState.Ignored,
                Username = targetPlayer.Username
            });
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        dbContext.Entry(ignore).State = EntityState.Added;
        await dbContext.SaveChangesAsync();
    }
}