using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

public class PlayerClubCenterDataEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var subscription = client.Player?.Data.Subscriptions.FirstOrDefault(x => x.Name == "HABBO_CLUB");
        
        if (subscription == null)
        {
            await client.WriteToStreamAsync(new PlayerClubCenterDataWriter(
                "",
                0,
                0.1,
                0,
                0,
                0,
                0,
                0,
                0).GetAllBytes());
            
            return;
        }
        
        await client.WriteToStreamAsync(new PlayerClubCenterDataWriter(
            subscription.Started.ToString("dd/MM/yyyy"),
            0,
            0.1,
            0,
            0,
            0,
            0,
            0,
            0).GetAllBytes());
    }
}