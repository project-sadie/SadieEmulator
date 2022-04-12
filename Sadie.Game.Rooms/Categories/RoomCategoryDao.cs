using Sadie.Database;

namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryDao : BaseDao, IRoomCategoryDao
{
    private readonly RoomCategoryFactory _categoryFactory;

    public RoomCategoryDao(IDatabaseProvider databaseProvider, RoomCategoryFactory categoryFactory) : base(databaseProvider)
    {
        _categoryFactory = categoryFactory;
    }
    
    public async Task<List<RoomCategory>> GetAllCategoriesAsync()
    {
        var categories = new List<RoomCategory>();
        var reader = await GetReaderAsync("SELECT `id`,`caption`,`is_visible` FROM `room_categories`;", new Dictionary<string, object>());

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            categories.Add(_categoryFactory.CreateFromRecord(record.Get<int>("id"), 
                record.Get<string>("caption"), 
                record.Get<int>("is_visible") == 1));
        }

        return categories;
    }
}