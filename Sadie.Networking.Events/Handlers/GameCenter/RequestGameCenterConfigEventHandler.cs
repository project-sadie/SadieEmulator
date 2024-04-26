using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.GameCentre;

namespace Sadie.Networking.Events.Handlers.GameCenter;

public class RequestGameCenterConfigEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.GameCenterRequestGames;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var writer = new GameAchievementsListWriter();
        await client.WriteToStreamAsync(writer);
    }
}