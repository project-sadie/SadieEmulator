using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerActivity)]
public class PlayerActivityEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient networkClient)
    {
        return Task.CompletedTask;
    }
}