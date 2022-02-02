using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Navigator;
using Sadie.Shared;

namespace Sadie.Networking.Packets.Client.Navigator;

public class NavigatorDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new NavigatorMetaDataWriter().GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorLiftedRoomsWriter().GetAllBytes());
        
        await client.WriteToStreamAsync(new NavigatorCollapsedCategoriesWriter(SadieConstants.NavigatorCategories).GetAllBytes());
        
        await client.WriteToStreamAsync(new NavigatorSavedSearchesWriter().GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorEventCategoriesWriter().GetAllBytes());
    }
}