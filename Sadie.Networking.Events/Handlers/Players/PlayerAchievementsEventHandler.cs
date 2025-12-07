using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

[PacketId(EventHandlerId.PlayerAchievements)]
public class PlayerAchievementsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new PlayerAchievementsWriter());
    }
}