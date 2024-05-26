using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Camera;

namespace Sadie.Networking.Events.Handlers.Camera;

[PacketId(EventHandlerIds.CameraPrice)]
public class CameraPriceEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
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