using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerIgnoredUsersEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerIgnoredUsers;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var writer = new PlayerIgnoredUsersWriter
        {
            IgnoredUsernames = []
        };
        
        await client.WriteToStreamAsync(writer);
    }
}