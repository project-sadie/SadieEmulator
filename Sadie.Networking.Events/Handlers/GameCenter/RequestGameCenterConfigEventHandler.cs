using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Writers.GameCentre;

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