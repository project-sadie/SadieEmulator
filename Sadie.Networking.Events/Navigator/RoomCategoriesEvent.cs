using Sadie.Game.Rooms.Categories;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Navigator;

public class RoomCategoriesEvent(IRoomCategoryRepository roomCategoryRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new RoomCategoriesWriter(
            roomCategoryRepository.GetAllCategories()
        ).GetAllBytes());
    }
}