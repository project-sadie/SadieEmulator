using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.GameCentre;

namespace Sadie.Networking.Events.Handlers.GameCenter;

public class RequestGameCenterConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new GameAchievementsListWriter().GetAllBytes());
    }
}