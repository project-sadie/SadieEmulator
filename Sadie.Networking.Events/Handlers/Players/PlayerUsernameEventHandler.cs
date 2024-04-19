using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerUsernameEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerUsername;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}