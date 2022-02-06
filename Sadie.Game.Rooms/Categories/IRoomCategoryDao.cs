namespace Sadie.Game.Rooms.Categories;

public interface IRoomCategoryDao
{
    Task<List<RoomCategory>> GetAllCategoriesAsync();
}