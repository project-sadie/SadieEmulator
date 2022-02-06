namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryRepository : IRoomCategoryRepository
{
    private List<RoomCategory> _categories;
    private readonly IRoomCategoryDao _categoryDao;

    public RoomCategoryRepository(IRoomCategoryDao categoryDao)
    {
        _categories = new List<RoomCategory>();
        _categoryDao = categoryDao;
    }

    public async Task LoadInitialDataAsync()
    {
        _categories = await _categoryDao.GetAllCategoriesAsync();
    }

    public List<RoomCategory> GetAllCategories()
    {
        return _categories;
    }
}