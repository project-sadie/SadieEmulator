using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players.Club;

public class PlayerClubMembershipEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var subscriptionName = reader.ReadString();
        var subscription = client.Player.Data.Subscriptions.FirstOrDefault(x => x.Name == subscriptionName);
        
        await client.WriteToStreamAsync(new PlayerClubMembershipWriter(subscriptionName, subscription).GetAllBytes());
    }
}