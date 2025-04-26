using Sadie.API.Game.Players;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Subscriptions;

namespace Sadie.Networking.Events.Handlers.Players.Club;

[PacketId(EventHandlerId.PlayerSubscription)]
public class PlayerSubscriptionEventHandler(IPlayerHelperService playerHelperService) : INetworkPacketEventHandler
{
    public string? Name { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (string.IsNullOrEmpty(Name) || client.Player == null)
        {
            return;
        }

        var writer = playerHelperService.GetSubscriptionWriterAsync(client.Player, Name);

        if (writer == null)
        {
            return;
        }
        
        await client.WriteToStreamAsync((PlayerSubscriptionWriter) writer);
        client.Player.State.LastSubscriptionModification = DateTime.Now;
    }
}