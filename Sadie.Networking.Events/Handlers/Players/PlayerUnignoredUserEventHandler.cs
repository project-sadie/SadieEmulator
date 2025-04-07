using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Enums.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerRemoveUserIgnore)]
public class PlayerRemoveUserIgnoreEventHandler(IPlayerRepository playerRepository,
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
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
        
        if (targetPlayer == null || player.Ignores.All(x => x.TargetPlayerId != targetPlayer.Id))
        {
            return;
        }

        var ignore = player.Ignores.FirstOrDefault(x => x.TargetPlayerId == targetPlayer.Id);

        if (ignore == null)
        {
            return;
        }

        player.Ignores.Remove(ignore);

        await player.NetworkObject.WriteToStreamAsync(
            new PlayerIgnoreStateWriter
            {
                State = (int) PlayerIgnoreState.NotIgnored,
                Username = targetPlayer.Username,
            });
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        dbContext.Entry(ignore).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
    }
}