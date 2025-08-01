using Sadie.Networking.Client;
using Sadie.Networking.Writers.GameCentre;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.GameCenter;

[PacketId(EventHandlerId.GameCenterRequestGames)]
public class RequestGameCenterConfigEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var writer = new PlayerGameAchievementsListWriter();
        await client.WriteToStreamAsync(writer);
    }
} 