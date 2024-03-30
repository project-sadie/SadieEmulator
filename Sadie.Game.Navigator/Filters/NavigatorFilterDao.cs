using Sadie.Database;

namespace Sadie.Game.Navigator.Filters;

public class NavigatorFilterDao(
    IDatabaseProvider databaseProvider,
    NavigatorFilterFactory filterFactory)
    : BaseDao(databaseProvider)
{
    public async Task<List<NavigatorFilter>> GetAllAsync()
    {
        var filters = new List<NavigatorFilter>();
        var reader = await GetReaderAsync("SELECT filter_name FROM navigator_search_filters;");

        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var filter = filterFactory.Create(record.Get<string>("filter_name"));
            
            filters.Add(filter);
        }

        return filters;
    }
}