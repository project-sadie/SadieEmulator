using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Client.Unknown;

public class UnknownEvent2 : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}