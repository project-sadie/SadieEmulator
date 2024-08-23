using Sadie.API.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Generic;

namespace Sadie.Networking.Events.Handlers.Generic;

[PacketId(EventHandlerId.PlayerRelationships)]
public class PlayerRelationshipsEventHandler(
    IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
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