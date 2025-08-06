using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerActivity)]
public class PlayerActivityEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient networkClient)
    {
        return Task.CompletedTask;
    }
}