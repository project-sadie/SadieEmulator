using Sadie.Database.LegacyAdoNet;
using Sadie.Game.Players.Subscriptions;

namespace Sadie.Game.Players.DaosToDrop;

public class PlayerSubscriptionDao(
    IDatabaseProvider databaseProvider,
    IPlayerSubscriptionFactory playerSubscriptionFactory)
    : BaseDao(databaseProvider), IPlayerSubscriptionDao
{
    public async Task<List<IPlayerSubscription>> GetSubscriptionsForPlayerAsync(int playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT
                subscriptions.name,
                player_subscriptions.subscription_id,
                player_subscriptions.created_at,
                player_subscriptions.expires_at

            FROM player_subscriptions
                INNER JOIN subscriptions ON subscriptions.id = subscription_id
            WHERE player_id = @playerId AND expires_at > NOW()", new Dictionary<string, object>
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
            
            data.Add(playerSubscriptionFactory.Create(
                record.Get<string>("name"),
                record.Get<DateTime>("created_at"),
                record.Get<DateTime>("expires_at")));
        }
        
        return data;
    }
}