using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Camera;

namespace Sadie.Networking.Events.Handlers.Camera;

public class CameraPriceEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var costCredits = 0;
        var costPoints = 0;
        var pointsType = 0;
        
        await client.WriteToStreamAsync(new CameraPriceWriter(costCredits, costPoints, pointsType).GetAllBytes());
    }
}