using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class ClientVariablesEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.ClientVariables;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader) => Task.CompletedTask;
}