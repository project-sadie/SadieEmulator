using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Club;

[PacketId(EventHandlerIds.PlayerSubscription)]
public class PlayerSubscriptionEventHandler : INetworkPacketEventHandler
{
    public string? Name { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (string.IsNullOrEmpty(Name) || client.Player == null)
        {
            return;
        }

        var writer = PlayerHelpersToClean.GetSubscriptionWriterAsync(client.Player, Name);

        if (writer == null)
        {
            return;
        }
        
        await client.WriteToStreamAsync(writer);
        client.Player.State.LastSubscriptionModification = DateTime.Now;
    }
}