using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerIds.PlayerActivity)]
public class PlayerActivityEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient networkClient)
    {
        return Task.CompletedTask;
    }
}