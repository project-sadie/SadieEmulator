using Sadie.Database;
using Sadie.Shared;

namespace Sadie.Game.Players;

public class PlayerDao : BaseDao, IPlayerDao
{
    private readonly IPlayerFactory _playerFactory;

    public PlayerDao(IDatabaseProvider databaseProvider, IPlayerFactory playerFactory) : base(databaseProvider)
    {
        _playerFactory = playerFactory;
    }

    public async Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoTokenAsync(string ssoToken)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   `players`.`id`, 
                   `players`.`username`, 
                   
                   `player_data`.`home_room_id`, 
                   `player_data`.`figure_code`, 
                   `player_data`.`motto`, 
                   `player_data`.`gender`, 
                   `player_data`.`credit_balance`, 
                   `player_data`.`pixel_balance`, 
                   `player_data`.`seasonal_balance`, 
                   `player_data`.`gotw_points`,
                   `player_data`.`respect_points`,
                   `player_data`.`respect_points_pet`,
                   `player_data`.`last_online`,
                   `player_data`.`achievement_score`,
            
                    `player_navigator_settings`.`window_x`,
                    `player_navigator_settings`.`window_y`,
                    `player_navigator_settings`.`window_width`,
                    `player_navigator_settings`.`window_height`,
                    `player_navigator_settings`.`open_searches`,

                   `player_game_settings`.`system_volume`,
                   `player_game_settings`.`furniture_volume`,
                   `player_game_settings`.`trax_volume`,
                   `player_game_settings`.`prefer_old_chat`,
                   `player_game_settings`.`block_room_invited`,
                   `player_game_settings`.`block_camera_follow`,
                   `player_game_settings`.`ui_flags`,
                   `player_game_settings`.`chat_color`,
                   `player_game_settings`.`show_notifications`,
                   
                    (SELECT COUNT(*) FROM `player_respects` WHERE `target_profile_id` = `players`.`id`) AS `respects_received`
            
            FROM `players` 
                INNER JOIN `player_data` ON `player_data`.`profile_id` = `players`.`id` 
                INNER JOIN `player_navigator_settings` ON `player_navigator_settings`.`profile_id` = `players`.`id` 
                INNER JOIN `player_game_settings` ON `player_game_settings`.`profile_id` = `players`.`id` 
        
            WHERE `players`.`sso_token` = @ssoToken LIMIT 1;", new Dictionary<string, object>
        {
            { "ssoToken", ssoToken }
        });

        var (success, record) = reader.Read();

        if (success && record != null)
        {
            var savedSearchesReader = await GetReaderForSavedSearchesAsync(record.Get<int>("id"));
            return new Tuple<bool, IPlayer?>(true, _playerFactory.CreateFromRecord(record, savedSearchesReader));
        }

        return new Tuple<bool, IPlayer?>(false, null);
    }

    private async Task<DatabaseReader> GetReaderForSavedSearchesAsync(long id)
    {
        return await GetReaderAsync("SELECT `id`,`search`,`filter` FROM `player_saved_searches` WHERE `profile_id` = @profileId", new Dictionary<string, object>
        {
            { "profileId", id }
        });
    }

    public async Task MarkPlayerAsOnlineAsync(long id)
    {
        await QueryAsync("UPDATE `player_data` SET `is_online` = 1, `last_online` = @lastOnline WHERE `profile_id` = @profileId", new Dictionary<string, object>
        {
            { "lastOnline", DateTime.Now.ToString(SadieConstants.DateTimeFormat) },
            { "profileId", id }
        });
    }

    public async Task MarkPlayerAsOfflineAsync(long id)
    {
        await QueryAsync("UPDATE `player_data` SET `is_online` = 0 WHERE `profile_id` = @profileId", new Dictionary<string, object>
        {
            { "profileId", id }
        });
    }

    public async Task ResetSsoTokenForPlayerAsync(long id)
    {
        await QueryAsync("UPDATE `players` SET `sso_token` = '' WHERE `id` = @profileId", new Dictionary<string, object>
        {
            { "profileId", id }
        });
    }
}