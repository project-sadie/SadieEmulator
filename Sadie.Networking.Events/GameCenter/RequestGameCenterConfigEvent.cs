using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.GameCentre;

namespace Sadie.Networking.Events.GameCenter;

public class RequestGameCenterConfigEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new GameAchievementsListWriter().GetAllBytes());
    }
}