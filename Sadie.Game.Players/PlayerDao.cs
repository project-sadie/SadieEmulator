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
        var reader = await GetReaderAsync("SELECT `players`.`id`, `player_data`.`username`, `player_data`.`home_room_id`, `player_data`.`figure_code`, `player_data`.`motto`, `player_data`.`gender` FROM `players` INNER JOIN `player_data` ON `player_data`.`profile_id` = `players`.`id` WHERE `players`.`sso_token` = @ssoToken LIMIT 1;", new Dictionary<string, object>
        {
            { "ssoToken", ssoToken }
        });

        var (success, record) = reader.Read();

        return success && record != null ?
            new Tuple<bool, IPlayer?>(true, PlayerFactory.CreateFromRecord(record)) : 
            new Tuple<bool, IPlayer?>(false, null);
    }

    public async Task MarkPlayerAsOnlineAsync(long id)
    {
        await QueryAsync("UPDATE `player_data` SET `online` = 1, `last_online` = @lastOnline WHERE `profile_id` = @profileId", new Dictionary<string, object>
        {
            { "lastOnline", DateTime.Now.ToString(SadieConstants.DateTimeFormat) },
            { "profileId", id }
        });
    }

    public async Task MarkPlayerAsOfflineAsync(long id)
    {
        await QueryAsync("UPDATE `player_data` SET `online` = 0 WHERE `profile_id` = @profileId", new Dictionary<string, object>
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