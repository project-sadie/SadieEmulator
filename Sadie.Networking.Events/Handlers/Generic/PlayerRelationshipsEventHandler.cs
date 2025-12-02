using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Generic;

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