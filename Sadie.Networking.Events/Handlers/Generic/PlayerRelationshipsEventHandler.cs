using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers.Generic;

[PacketId(EventHandlerIds.PlayerRelationships)]
public class PlayerRelationshipsEventHandler(
    PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var playerId = eventParser.PlayerId;
        var player = await playerRepository.GetPlayerByIdAsync(playerId);

        var relationships = player != null ? 
                player.Relationships : 
                await playerRepository.GetRelationshipsForPlayerAsync(playerId);

        await client.WriteToStreamAsync(new PlayerRelationshipsWriter
        {
            PlayerId = playerId,
            Relationships = relationships
        });
    }
}