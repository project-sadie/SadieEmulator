using Sadie.Database;
using Sadie.Shared;

namespace Sadie.Game.Players;

public class PlayerDao : BaseDao, IPlayerDao
{
    public PlayerDao(IDatabaseProvider databaseProvider) : base(databaseProvider)
    {
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
            
                    `player_navigator_settings`.`window_x`,
                    `player_navigator_settings`.`window_y`,
                    `player_navigator_settings`.`window_width`,
                    `player_navigator_settings`.`window_height`,
                    `player_navigator_settings`.`open_searches`
            
            FROM `players` 
                INNER JOIN `player_data` ON `player_data`.`profile_id` = `players`.`id` 
                INNER JOIN `player_navigator_settings` ON `player_navigator_settings`.`profile_id` = `players`.`id` 
            WHERE `players`.`sso_token` = @ssoToken LIMIT 1;", new Dictionary<string, object>
        {
            { "ssoToken", ssoToken }
        });

        var (success, record) = reader.Read();

        if (success && record != null)
        {
            var savedSearchesReader = await GetReaderForSavedSearchesAsync(record.Get<int>("id"));
            return new Tuple<bool, IPlayer?>(true, PlayerFactory.CreateFromRecord(record, savedSearchesReader));
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