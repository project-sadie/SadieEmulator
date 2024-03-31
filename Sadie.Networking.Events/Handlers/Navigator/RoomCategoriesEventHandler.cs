using Sadie.Game.Rooms.Categories;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Navigator;

public class RoomCategoriesEventHandler(IRoomCategoryRepository roomCategoryRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomCategories;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new RoomCategoriesWriter(
            roomCategoryRepository.GetAllCategories()
        ).GetAllBytes());
    }
}