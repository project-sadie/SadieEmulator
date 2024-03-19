namespace Sadie.Game.Rooms.FurnitureItems;

public interface IRoomFurnitureItemDao
{
    Task<int> CreateItemAsync(RoomFurnitureItem item);
    Task<List<RoomFurnitureItem>> GetItemsForRoomAsync(int roomId);
    Task DeleteItemsAsync(List<long> itemIds);
}