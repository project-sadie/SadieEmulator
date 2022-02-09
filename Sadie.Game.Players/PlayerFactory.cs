using Sadie.Database;
using Sadie.Game.Players.Avatar;
using Sadie.Game.Players.Navigator;

namespace Sadie.Game.Players;

public class PlayerFactory
{
    private static PlayerBalance CreateBalanceFromRecord(DatabaseRecord record)
    {
        return new PlayerBalance(
            record.Get<long>("credit_balance"),
            record.Get<long>("pixel_balance"),
            record.Get<long>("seasonal_balance"),
            record.Get<long>("gotw_points"));
    }

    private static List<PlayerSavedSearch> CreateSavedSearchesFromReader(DatabaseReader savedSearchesReader)
    {
        var data = new List<PlayerSavedSearch>();
        
        while (true)
        {
            var (success, record) = savedSearchesReader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(new PlayerSavedSearch(
                record.Get<long>("id"),
                record.Get<string>("search"),
                record.Get<string>("filter")));
        }
        
        return data;
    }
    
    public static IPlayer CreateFromRecord(DatabaseRecord record, DatabaseReader savedSearchesReader)
    {
        return new Player(
            record.Get<long>("id"),
            record.Get<string>("username"),
            record.Get<long>("home_room_id"),
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? PlayerAvatarGender.Male : PlayerAvatarGender.Female,
            CreateBalanceFromRecord(record),
            DateTime.TryParse(record.Get<string>("last_online"), out var timestamp) ? timestamp : DateTime.MinValue,
            0, // TODO: load this
            record.Get<long>("respect_points"),
            record.Get<long>("respect_points_pet"),
            CreateNavigatorSettingsFromRecord(record),
            CreateSettingsFromRecord(), 
            CreateSavedSearchesFromReader(savedSearchesReader)
        );
    }

    private static PlayerNavigatorSettings CreateNavigatorSettingsFromRecord(DatabaseRecord record)
    {
        return new PlayerNavigatorSettings(
            record.Get<int>("window_x"),
            record.Get<int>("window_y"),
            record.Get<int>("window_width"),
            record.Get<int>("window_height"),
            record.Get<int>("open_searches") == 1,
            0);
    }

    private static PlayerSettings CreateSettingsFromRecord()
    {
        // TODO: Load this
        return new PlayerSettings(100, 100, 100, false, false, false, 0, 1);
    }
}