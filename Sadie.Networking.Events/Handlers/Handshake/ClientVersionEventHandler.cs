using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class ClientVersionEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.ClientVersion;

    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader) => Task.CompletedTask;
}