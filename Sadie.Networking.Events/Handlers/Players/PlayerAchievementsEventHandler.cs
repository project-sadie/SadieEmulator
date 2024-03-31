using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerAchievementsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerAchievements;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerAchievementsWriter().GetAllBytes());
    }
}