using Sadie.Database;
using Sadie.Shared.Game.Avatar;

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
                   `players`.`role_id`, 
                   
                   `player_data`.`home_room_id`, 
                   `player_data`.`credit_balance`, 
                   `player_data`.`pixel_balance`, 
                   `player_data`.`seasonal_balance`, 
                   `player_data`.`gotw_points`,
                   `player_data`.`respect_points`,
                   `player_data`.`respect_points_pet`,
                   `player_data`.`last_online`,
                   `player_data`.`achievement_score`,
                   
                   `player_avatar_data`.`figure_code`, 
                   `player_avatar_data`.`motto`, 
                   `player_avatar_data`.`gender`, 
                   
                   (SELECT GROUP_CONCAT(`name`) AS `comma_seperated_tags`
                    FROM `player_tags`
                    WHERE `player_id` = `players`.`id`
                    GROUP BY `player_id`) AS `comma_seperated_tags`,
            
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
                   
                    (SELECT COUNT(*) FROM `player_respects` WHERE `target_player_id` = `players`.`id`) AS `respects_received`
            
            FROM `players` 
                INNER JOIN `player_data` ON `player_data`.`player_id` = `players`.`id` 
                INNER JOIN `player_avatar_data` ON `player_avatar_data`.`player_id` = `players`.`id` 
                INNER JOIN `player_navigator_settings` ON `player_navigator_settings`.`player_id` = `players`.`id` 
                INNER JOIN `player_game_settings` ON `player_game_settings`.`player_id` = `players`.`id` 
        
            WHERE `players`.`sso_token` = @ssoToken LIMIT 1;", new Dictionary<string, object>
        {
            { "ssoToken", ssoToken }
        });

        var (success, record) = reader.Read();

        if (!success || record == null)
        {
            return new Tuple<bool, IPlayer?>(false, null);
        }
        
        var savedSearchesReader = await GetReaderForSavedSearchesAsync(record.Get<int>("id"));
        var permissionsReader = await GetReaderForPermissionsAsync(record.Get<int>("role_id"));
            
        return new Tuple<bool, IPlayer?>(true, _playerFactory.CreateFromRecord(record, savedSearchesReader, permissionsReader));
    }

    private async Task<DatabaseReader> GetReaderForSavedSearchesAsync(long id)
    {
        return await GetReaderAsync("SELECT `id`,`search`,`filter` FROM `player_saved_searches` WHERE `player_id` = @profileId", new Dictionary<string, object>
        {
            { "profileId", id }
        });
    }

    private async Task<DatabaseReader> GetReaderForPermissionsAsync(long roleId)
    {
        return await GetReaderAsync("SELECT `name` FROM `player_permissions` WHERE `id` IN (SELECT `permission_id` FROM `player_role_permissions` WHERE `player_role_permissions`.`role_id` = @roleId)", new Dictionary<string, object>
        {
            { "roleId", roleId }
        });
    }

    public async Task MarkPlayerAsOnlineAsync(long id)
    {
        await QueryAsync("UPDATE `player_data` SET `is_online` = 1, `last_online` = @lastOnline WHERE `player_id` = @profileId", new Dictionary<string, object>
        {
            { "lastOnline", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
            { "profileId", id }
        });
    }

    public async Task MarkPlayerAsOfflineAsync(IPlayer player)
    {
        await QueryAsync(@"UPDATE `player_data` 
            SET 
                `is_online` = 0, 
                `figure_code` = @figureCode, 
                `motto` = @motto, 
                `gender` = @gender, 
                `credit_balance` = @creditBalance, 
                `pixel_balance` = @pixelBalance,
                `seasonal_balance` = @seasonalBalance,
                `gotw_points` = @gotwPoints,
                `respect_points` = @respectPoints,
                `respect_points_pet` = @respectPointsPet,
                `achievement_score` = @achievementScore
            WHERE `player_id` = @playerId", new Dictionary<string, object>
        {
            { "figureCode", player.FigureCode },
            { "motto", player.Motto },
            { "gender", player.Gender == AvatarGender.Male ? "M" : "F" },
            { "creditBalance", player.Balance.Credits },
            { "pixelBalance", player.Balance.Pixels },
            { "seasonalBalance", player.Balance.Seasonal },
            { "gotwPoints", player.Balance.Gotw },
            { "respectPoints", player.RespectPoints },
            { "respectPointsPet", player.RespectPointsPet },
            { "achievementScore", player.AchievementScore },
            { "playerId", player.Id }
        });
    }

    public async Task ResetSsoTokenForPlayerAsync(long id)
    {
        await QueryAsync("UPDATE `players` SET `sso_token` = '' WHERE `id` = @playerId", new Dictionary<string, object>
        {
            { "playerId", id }
        });
    }
}