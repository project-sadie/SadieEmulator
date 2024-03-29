using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Club;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Players.Club;

public class PlayerClubMembershipEvent(PlayerClubMembershipParser parser) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var subscriptionName = reader.ReadString();
        var subscription = client.Player.Data.Subscriptions.FirstOrDefault(x => x.Name == subscriptionName);
        
        await client.WriteToStreamAsync(new PlayerClubMembershipWriter(subscriptionName, subscription).GetAllBytes());
    }
}