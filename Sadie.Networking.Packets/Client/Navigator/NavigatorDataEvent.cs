using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Navigator;

namespace Sadie.Networking.Packets.Client.Navigator;

public class NavigatorDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new NavigatorMetaDataParser().GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorLiftedRooms().GetAllBytes());
        await client.WriteToStreamAsync(new NewNavigatorCollapsedCategories().GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorSavedSearches().GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorEventCategories().GetAllBytes());
    }
}