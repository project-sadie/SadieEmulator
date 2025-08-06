using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerIgnoredUsers)]
public class PlayerIgnoredUsersEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var writer = new PlayerIgnoredUsersWriter
        {
            IgnoredUsernames = []
        };
        
        await client.WriteToStreamAsync(writer);
    }
}