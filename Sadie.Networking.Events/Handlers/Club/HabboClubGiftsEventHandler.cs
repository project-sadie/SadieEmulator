using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Other;

namespace Sadie.Networking.Events.Handlers.Club;

public class HabboClubGiftsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.HabboClubGifts;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new HabboClubGiftsWriter(1, 1).GetAllBytes());
    }
}