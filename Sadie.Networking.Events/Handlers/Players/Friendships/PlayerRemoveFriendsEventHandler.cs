using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerIds.PlayerRemoveFriends)]
public class PlayerRemoveFriendsEventHandler(
    PlayerRemoveFriendsEventParser eventParser,
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = client.Player.Id;
        
        for (var i = 0; i < eventParser.Amount; i++)
        {
            var currentId = eventParser.Ids[i];
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
                x.OriginPlayerId == playerId && eventParser.Ids.Contains(x.TargetPlayerId) || 
                x.TargetPlayerId == playerId && eventParser.Ids.Contains(x.OriginPlayerId))
            .ExecuteDeleteAsync();
        
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter
        {
            Unknown1 = 0,
            PlayerIds = eventParser.Ids
        });
    }
}