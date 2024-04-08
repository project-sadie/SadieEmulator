using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerRemoveFriendsEventHandler(
    PlayerRemoveFriendsEventParser eventParser,
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerRemoveFriends;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = client.Player.Id;
        
        for (var i = 0; i < eventParser.Amount; i++)
        {
            var currentId = eventParser.Ids[i];

            if (!playerRepository.TryGetPlayerById(currentId, out var target) || target == null)
            {
                continue;
            }
            
            var friendship = target
                .Friendships
                .FirstOrDefault(x => x.OriginPlayerId == playerId || x.TargetPlayerId == playerId);

            if (friendship != null)
            {
                target.Friendships.Remove(friendship);
            }
                
            await target.NetworkObject.WriteToStreamAsync(new PlayerRemoveFriendsWriter([playerId]).GetAllBytes());
        }

        client
            .Player
            .Friendships
            .RemoveAll(x => x.OriginPlayerId == playerId || x.TargetPlayerId == playerId);

        await dbContext
            .Set<PlayerFriendship>()
            .Where(x => x.OriginPlayerId == playerId && eventParser.Ids.Contains(x.TargetPlayerId) || x.TargetPlayerId == playerId && eventParser.Ids.Contains(x.OriginPlayerId))
            .ExecuteDeleteAsync();
        
        await client.WriteToStreamAsync(new PlayerRemoveFriendsWriter(eventParser.Ids).GetAllBytes());
    }
}