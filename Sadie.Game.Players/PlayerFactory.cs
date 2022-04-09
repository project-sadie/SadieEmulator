using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Database;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public class PlayerFactory : IPlayerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PlayerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private IPlayerBalance CreateBalanceFromRecord(DatabaseRecord record)
    {
        return ActivatorUtilities.CreateInstance<PlayerBalance>(
            _serviceProvider, 
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

    private static List<string> CreatePermissionsFromReader(DatabaseReader reader)
    {
        var data = new List<string>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(record.Get<string>("name"));
        }
        
        return data;
    }
    
    public IPlayer Create(DatabaseRecord record, DatabaseReader savedSearchesReader, DatabaseReader permissionsReader, List<PlayerBadge> badges, List<PlayerFriendshipData> friendships)
    {
        return ActivatorUtilities.CreateInstance<Player>(
            _serviceProvider,
            _serviceProvider.GetRequiredService<ILogger<Player>>(),
            _serviceProvider.GetRequiredService<IPlayerRepository>(),
            record.Get<int>("id"),
            record.Get<string>("username"),
            record.Get<DateTime>("created_at"),
            record.Get<long>("home_room_id"),
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female,
            CreateBalanceFromRecord(record),
            DateTime.TryParse(record.Get<string>("last_online"), out var timestamp) ? timestamp : DateTime.MinValue,
            record.Get<int>("respects_received"),
            record.Get<int>("respect_points"),
            record.Get<int>("respect_points_pet"),
            CreateNavigatorSettingsFromRecord(record),
            CreateSettingsFromRecord(record),
            CreateSavedSearchesFromReader(savedSearchesReader),
            CreatePermissionsFromReader(permissionsReader),
            record.Get<long>("achievement_score"),
            new List<string>(record.Get<string>("comma_seperated_tags").Split(",")),
            badges,
            friendships);
    }
    
    public IPlayer CreateBasic(DatabaseRecord record)
    {
        return ActivatorUtilities.CreateInstance<Player>(
            _serviceProvider,
            _serviceProvider.GetRequiredService<ILogger<Player>>(),
            _serviceProvider.GetRequiredService<IPlayerRepository>(),
            record.Get<int>("id"),
            record.Get<string>("username"),
            record.Get<DateTime>("created_at"),
            0,
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female,
            new PlayerBalance(0, 0, 0, 0),
            DateTime.TryParse(record.Get<string>("last_online"), out var timestamp) ? timestamp : DateTime.MinValue,
            0,
            0,
            0,
            CreateNavigatorSettingsFromRecord(record),
            CreateSettingsFromRecord(record),
            new List<PlayerSavedSearch>(),
            new List<string>(),
            0,
            new List<string>());
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

    private static PlayerSettings CreateSettingsFromRecord(DatabaseRecord record)
    {
        return new PlayerSettings(
            record.Get<int>("system_volume"),
            record.Get<int>("furniture_volume"),
            record.Get<int>("trax_volume"),
            record.Get<int>("prefer_old_chat") == 1,
            record.Get<int>("block_room_invited") == 1,
            record.Get<int>("block_camera_follow") == 1,
            record.Get<int>("ui_flags"),
            record.Get<int>("chat_color"),
            record.Get<int>("show_notifications") == 1);
    }
}