namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryRepository(IRoomCategoryDao categoryDao) : IRoomCategoryRepository
{
    private List<RoomCategory> _categories = new();

    public async Task LoadInitialDataAsync()
    {
        _categories = await categoryDao.GetAllCategoriesAsync();
    }

    public List<RoomCategory> GetAllCategories()
    {
        return _categories;
    }
}