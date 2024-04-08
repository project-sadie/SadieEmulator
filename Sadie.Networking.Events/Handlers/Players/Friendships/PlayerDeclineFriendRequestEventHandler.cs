using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Friendships;
using Sadie.Networking.Packets;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

public class PlayerDeclineFriendRequestEventHandler(
    PlayerDeclineFriendRequestEventParser eventParser,
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerDeclineFriendRequest;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var player = client.Player;
        var playerData = player.Data;
        var playerId = player.Id;

        if (eventParser.DeclineAll)
        {
            await dbContext.Set<PlayerFriendship>()
                .Where(x => x.TargetPlayerId == playerId && x.Status == PlayerFriendshipStatus.Pending)
                .ExecuteDeleteAsync();
        }
        else
        {
            foreach (var originId in eventParser.Ids) 
            {
                var targetId = playerId;
                
                await dbContext.Set<PlayerFriendship>()
                    .Where(x => x.OriginPlayerId == originId && x.TargetPlayerId == targetId)
                    .ExecuteDeleteAsync();

                if (!playerRepository.TryGetPlayerById(originId, out var origin) || origin == null)
                {
                    continue;
                }
                
                var request = origin.Friendships.FirstOrDefault(x => x.TargetPlayerId == targetId);

                if (request != null)
                {
                    origin.Friendships.Remove(request);
                }
            }
        }
    }
}