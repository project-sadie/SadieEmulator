using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.GameCentre;

namespace Sadie.Networking.Events.Handlers.GameCenter;

[PacketId(EventHandlerIds.GameCenterRequestGames)]
public class RequestGameCenterConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var writer = new GameAchievementsListWriter();
        await client.WriteToStreamAsync(writer);
    }
} 