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
    public int PlayerId { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = await playerRepository.GetPlayerByIdAsync(PlayerId);

        var relationships = player != null ? 
                player.Relationships : 
                await playerRepository.GetRelationshipsForPlayerAsync(PlayerId);

        await client.WriteToStreamAsync(new PlayerRelationshipsWriter
        {
            PlayerId = PlayerId,
            Relationships = relationships
        });
    }
}