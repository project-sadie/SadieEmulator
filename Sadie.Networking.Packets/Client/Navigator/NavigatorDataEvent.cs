using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Navigator;
using Sadie.Shared;

namespace Sadie.Networking.Packets.Client.Navigator;

public class NavigatorDataEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var metaData = new Dictionary<string, int>
        {
            {"official_view", 0},
            {"hotel_view", 0},
            {"roomads_view", 0},
            {"myworld_view", 0}
        };
        
        await client.WriteToStreamAsync(new NavigatorMetaDataWriter(metaData).GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorLiftedRoomsWriter(new List<RoomEntity>()).GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorCollapsedCategoriesWriter(SadieConstants.NavigatorCategories).GetAllBytes());
        await client.WriteToStreamAsync(new NavigatorSavedSearchesWriter().GetAllBytes());

    }
}