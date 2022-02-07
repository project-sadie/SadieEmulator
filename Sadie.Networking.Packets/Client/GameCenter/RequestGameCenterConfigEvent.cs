using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.GameCentre;

namespace Sadie.Networking.Packets.Client.GameCenter;

public class RequestGameCenterConfigEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new GameAchievementsListWriter().GetAllBytes());
    }
}