using Sadie.Game.Rooms.Categories;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Navigator;

public class RoomCategoriesEvent : INetworkPacketEvent
{
    private readonly IRoomCategoryRepository _roomCategoryRepository;

    public RoomCategoriesEvent(IRoomCategoryRepository roomCategoryRepository)
    {
        _roomCategoryRepository = roomCategoryRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new RoomCategoriesWriter(
            _roomCategoryRepository.GetAllCategories()
        ).GetAllBytes());
    }
}