using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerPongEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerPong;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        client.LastPing = DateTime.Now;
    }
}