using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
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