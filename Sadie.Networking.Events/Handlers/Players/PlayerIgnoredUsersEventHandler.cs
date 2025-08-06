using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players;
using Sadie.Shared.Attributes;

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