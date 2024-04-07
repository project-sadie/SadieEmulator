using Sadie.Database.LegacyAdoNet;
using Sadie.Game.Players.Messenger;

namespace Sadie.Game.Players.DaosToDrop;

public class PlayerMessageDao(IDatabaseProvider databaseProvider) : BaseDao(databaseProvider), IPlayerMessageDao
{
    public async Task<int> CreateMessageAsync(PlayerMessage message)
    {
        return await QueryAsync(@"
            INSERT INTO player_messages (
                origin_player_id, 
                target_player_id, 
                message, 
                created_at
            ) 
            VALUES (@originId, @targetId, @message, @createdAt);", new Dictionary<string, object>
        {
            {"originId", message.OriginId},
            {"targetId", message.TargetId},
            {"message", message.Message},
            {"createdAt", message.CreatedAt}
        });
    }
}