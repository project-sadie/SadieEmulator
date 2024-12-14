using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Friendships;

[PacketId(EventHandlerId.PlayerDeclineFriendRequest)]
public class PlayerDeclineFriendRequestEventHandler(
    IPlayerService playerService,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public bool DeclineAll { get; set; }
    public required List<int> Ids { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;
        var playerId = player.Id;

        if (DeclineAll)
        {
            player.IncomingFriendships.Clear();
            
            await dbContext.Set<PlayerFriendship>()
                .Where(x => x.TargetPlayerId == playerId && x.Status == PlayerFriendshipStatus.Pending)
                .ExecuteDeleteAsync();
        }
        else
        {
            foreach (var originId in Ids) 
            {
                var targetId = playerId;
                
                await dbContext.Set<PlayerFriendship>()
                    .Where(x => x.OriginPlayerId == originId && x.TargetPlayerId == targetId)
                    .ExecuteDeleteAsync();

                var origin = await playerService.GetPlayerByIdAsync(originId);
                var request = origin?.OutgoingFriendships.FirstOrDefault(x => x.TargetPlayerId == targetId);

                if (request != null)
                {
                    origin?.OutgoingFriendships.Remove(request);
                }
            }
        }
    }
}