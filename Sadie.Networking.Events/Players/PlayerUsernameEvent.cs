using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerUsernameEvent : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}