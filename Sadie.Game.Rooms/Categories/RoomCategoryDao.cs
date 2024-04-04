using Sadie.Database.LegacyAdoNet;

namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryDao(IDatabaseProvider databaseProvider, RoomCategoryFactory categoryFactory)
    : BaseDao(databaseProvider), IRoomCategoryDao
{
    public async Task<List<RoomCategory>> GetAllCategoriesAsync()
    {
        var categories = new List<RoomCategory>();
        var reader = await GetReaderAsync("SELECT id,caption,is_visible FROM room_categories;", new Dictionary<string, object>());

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            categories.Add(categoryFactory.CreateFromRecord(record.Get<int>("id"), 
                record.Get<string>("caption"), 
                record.Get<int>("is_visible") == 1));
        }

        return categories;
    }
}