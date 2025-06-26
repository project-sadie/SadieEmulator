using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Camera;

namespace Sadie.Networking.Events.Handlers.Camera;

[PacketId(EventHandlerId.CameraPrice)]
public class CameraPriceEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        const int costCredits = 0;
        const int costPoints = 0;
        const int pointsType = 0;
        
        await client.WriteToStreamAsync(new CameraPriceWriter
        {
            CostCredits = costCredits,
            CostPoints = costPoints,
            PointsType = pointsType
        });
    }
}