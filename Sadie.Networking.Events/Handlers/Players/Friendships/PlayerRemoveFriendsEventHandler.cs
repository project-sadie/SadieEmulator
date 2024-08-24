using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerRemoveFriends)]
public class PlayerRemoveFriendsEventHandler(
    IPlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int Amount { get; set; }
    public List<int> Ids { get; init; } = [];
    
    public async Task HandleAsync(INetworkClient client)
    {
        var playerId = client.Player.Id;
        
        for (var i = 0; i < Amount; i++)
        {
            var currentId = Ids[i];
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
                
            await target.NetworkObject.WriteToStreamAsync(new PlayerRemoveFriendsWriter
            {
                Unknown1 = 0,
                PlayerIds = [playerId]
            });
            
            client.Player.DeleteFriendshipFor(currentId);
        }

        await dbContext
            .Set<PlayerFriendship>()
            .Where(x => 
                x.OriginPlayerId == playerId && Ids.Contains(x.TargetPlayerId) || 
                x.TargetPlayerId == playerId && Ids.Contains(x.OriginPlayerId))
            .ExecuteDeleteAsync();
        
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter
        {
            Unknown1 = 0,
            PlayerIds = Ids
        });
    }
}