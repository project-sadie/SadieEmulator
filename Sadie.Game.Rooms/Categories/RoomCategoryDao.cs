using Sadie.Database;

namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryDao : BaseDao, IRoomCategoryDao
{
    public RoomCategoryDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
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
            
            categories.Add(RoomCategoryFactory.CreateFromRecord(record));
        }

        return categories;
    }
}