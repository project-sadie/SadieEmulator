using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Networking.Packets.Writers.Players;

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
            player.Player.Ignores.Any(x => x.TargetPlayerId == targetPlayer.Player.Id))
        {
            return;
        }

        var ignore = new PlayerIgnoreDto
        {
            PlayerId = player.Player.Id,
            TargetPlayerId = targetPlayer.Player.Id
        };

        player.Player.Ignores.Add(ignore);

        await player.NetworkObject.WriteToStreamAsync(
            new PlayerIgnoreStateWriter
            {
                State = (int) PlayerIgnoreState.Ignored,
                Username = targetPlayer.Player.Username
            });
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        dbContext.Entry(ignore).State = EntityState.Added;
        await dbContext.SaveChangesAsync();
    }
}