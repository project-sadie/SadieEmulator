using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerRemoveFriends)]
public class PlayerRemoveFriendsEventHandler(
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory)
    : INetworkPacketEventHandler
{
    public List<long> Ids { get; init; } = [];
    
    public async Task HandleAsync(INetworkClient client)
    {
        var playerId = client.Player.Id;
        
        foreach (var currentId in Ids)
        {
            var target = playerRepository.GetPlayerLogicById(currentId);
            
            if (target == null)
            {
                continue;
            }

            var friendship = target.TryGetAcceptedFriendshipFor(target.Id);

            if (friendship != null)
            {
                target.DeleteFriendshipFor(playerId);
            }
                
            await target.NetworkObject!.WriteToStreamAsync(new PlayerRemoveFriendsWriter
            {
                Unknown1 = 0,
                PlayerIds = [playerId]
            });
            
            client.Player.DeleteFriendshipFor(currentId);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();

        foreach (var currentId in Ids)
        {
            await dbContext
                .Set<PlayerFriendship>()
                .Where(x => 
                    x.OriginPlayerId == currentId && x.TargetPlayerId == playerId ||
                    x.TargetPlayerId == currentId && x.OriginPlayerId == playerId)
                .ExecuteDeleteAsync();
        }
        
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter
        {
            Unknown1 = 0,
            PlayerIds = Ids
        });
    }
}