using Sadie.Database;

namespace Sadie.Game.Players.Subscriptions;

public class PlayerSubscriptionDao : BaseDao, IPlayerSubscriptionDao
{
    private readonly IPlayerSubscriptionFactory _playerSubscriptionFactory;

    public PlayerSubscriptionDao(IDatabaseProvider databaseProvider, IPlayerSubscriptionFactory playerSubscriptionFactory) : base(databaseProvider)
    {
        _playerSubscriptionFactory = playerSubscriptionFactory;
    }

    public async Task<List<IPlayerSubscription>> GetSubscriptionsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT
                `subscriptions`.`name`,
                `player_subscriptions`.`subscription_id`,
                `player_subscriptions`.`created_at`,
                `player_subscriptions`.`expires_at`

            FROM player_subscriptions
                INNER JOIN `subscriptions` ON `subscriptions`.`id` = `subscription_id`
            WHERE `player_id` = @playerId AND `expires_at` < NOW()", new Dictionary<string, object>
        {
            {"playerId", playerId}
        });
        
        var data = new List<IPlayerSubscription>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(_playerSubscriptionFactory.Create(
                record.Get<string>("name"),
                record.Get<DateTime>("created_at"),
                record.Get<DateTime>("expires_at")));
        }
        
        return data;
    }
}