using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Generic;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers.Generic;

public class PlayerRelationshipsEventHandler(
    PlayerRelationshipsEventParser eventParser,
    PlayerRepository playerRepository,
    SadieContext dbContext) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerRelationships;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;
        var player = await playerRepository.GetPlayerByIdAsync(playerId);

        var relationships = player != null ? 
                player.Relationships : 
                await playerRepository.GetRelationshipsForPlayerAsync(playerId);

        var playerFriends = player != null
            ? player.GetMergedFriendships()
            : await dbContext
                .Set<PlayerFriendship>()
                .Where(x => x.OriginPlayerId == playerId || x.TargetPlayerId == playerId)
                .ToListAsync();
        
        await client.WriteToStreamAsync(new PlayerRelationshipsWriter(playerId, 
            relationships));
    }
}