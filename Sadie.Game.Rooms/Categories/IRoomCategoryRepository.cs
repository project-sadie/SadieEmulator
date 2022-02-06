namespace Sadie.Game.Rooms.Categories;

public interface IRoomCategoryRepository
{
    Task LoadInitialDataAsync();
    List<RoomCategory> GetAllCategories();
}