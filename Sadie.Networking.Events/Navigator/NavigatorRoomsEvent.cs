using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Navigator;

public class NavigatorRoomsEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var tabName = reader.ReadString();
        var searchQuery = reader.ReadString();

        await client.WriteToStreamAsync(new NavigatorRoomsWriter(tabName, searchQuery).GetAllBytes());
    }
}